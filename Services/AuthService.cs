using DevJobsBackend.Contracts.Services;
using DevJobsBackend.Data;
using DevJobsBackend.Dtos;
using DevJobsBackend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DevJobsBackend.Services
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;


        public AuthService(DataContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;

        }

        public bool CompareHashPassword(string UserPassword, string DatabasePassword)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(UserPassword);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                string hashedUserPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                return hashedUserPassword == DatabasePassword;
            }
        }

        public Task<string> GenerateHashPassword(string password)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(password);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                string hashedPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                return Task.FromResult(hashedPassword);
            }
        }

        public  string RefreshJwtToken(string refreshToken)
{
    // Validar e ler o token de atualização
    var tokenHandler = new JwtSecurityTokenHandler();
    var validationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:RefreshTokenSecret"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = _configuration["AppSettings:Issuer"],
        ValidAudience = _configuration["AppSettings:Audience"],
        ClockSkew = TimeSpan.Zero
    };

    SecurityToken validatedToken;
    ClaimsPrincipal principal;
    try
    {
        principal = tokenHandler.ValidateToken(refreshToken, validationParameters, out validatedToken);
    }
    catch (Exception ex)
    {
        // Token de atualização inválido
        throw new SecurityTokenException("Invalid refresh token", ex);
    }

    // Extraia informações do token de atualização
    var emailClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
    if (string.IsNullOrEmpty(emailClaim))
    {
        throw new SecurityTokenException("Invalid refresh token: missing email claim");
    }

    // Verifique se o token de atualização ainda é válido
    if (validatedToken.ValidTo < DateTime.UtcNow)
    {
        throw new SecurityTokenException("Refresh token has expired");
    }

    // Gere um novo token JWT com base nas informações do usuário
    var jwtToken = GenerateJwtToken(emailClaim);

    return jwtToken;
}


        public string GenerateJwtToken(string email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:SecretToken"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
        new Claim(JwtRegisteredClaimNames.Sub, email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    };

            var token = new JwtSecurityToken(
                issuer: _configuration["AppSettings:Issuer"],
                audience: _configuration["AppSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<ResponseModel<TokenResponseModel>> Login(LoginDTO loginDTO)
        {
            ResponseModel<TokenResponseModel> response = new ResponseModel<TokenResponseModel>();

            var UserFromDatabase = await _context.Users.FirstOrDefaultAsync(UserData => UserData.Email == loginDTO.Email);

            if (UserFromDatabase == null)
            {
                response.Message = "Email or Password incorrect";
                return response;
            }

            bool IsCorrectPassword = this.CompareHashPassword(loginDTO.Password, UserFromDatabase.HashPassword);

            if (!IsCorrectPassword)
            {
                response.Message = "Email or Password incorrect";
                return response;
            }

            var JwtToken = GenerateJwtToken(loginDTO.Email);





            throw new NotImplementedException();
        }

        public async Task<dynamic> RegistrateUser(User user)
        {
            _context.Users.Add(user);
            return await _context.SaveChangesAsync() > 0
                ? user
                : (dynamic)"Unable to register user";
        }

    }
}
