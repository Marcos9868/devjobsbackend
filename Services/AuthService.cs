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
            _context = context;
            _configuration = configuration;
        }

        private bool CompareHashPassword(string userPassword, string databasePassword)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(userPassword);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                string hashedUserPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                return hashedUserPassword == databasePassword;
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

        private TokenResponseModel GenerateAccessToken(string refreshToken)
        {
            var email = ValidateRefreshToken(refreshToken);
            var accessToken = GenerateJwtToken(email);
            var RenovatedRefreshToken = GenerateRefreshToken(email);
            TokenResponseModel Tokens = new TokenResponseModel
            {
                AccessToken = accessToken,
                RefreshToken = RenovatedRefreshToken
            };
            return Tokens;
        }

        private string ValidateRefreshToken(string refreshToken)
        {
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
                throw new SecurityTokenException("Invalid refresh token", ex);
            }

            var emailClaim = principal.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(emailClaim))
            {
                throw new SecurityTokenException("Invalid refresh token: missing email claim");
            }

            if (validatedToken.ValidTo < DateTime.UtcNow)
            {
                throw new SecurityTokenException("Refresh token has expired");
            }

            return emailClaim;
        }

        public string GenerateJwtToken(string email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:SecretToken"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
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

        public string GenerateRefreshToken(string email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:RefreshTokenSecret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        // Adicionar uma reivindicação de email ao token
        new Claim(JwtRegisteredClaimNames.Email, email)
    };

            var token = new JwtSecurityToken(
                issuer: _configuration["AppSettings:Issuer"],
                audience: _configuration["AppSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7), // Define a expiração do refreshToken (por exemplo, 7 dias)
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<ResponseModel<TokenResponseModel>> Login(LoginDTO loginDTO)
        {
            ResponseModel<TokenResponseModel> response = new ResponseModel<TokenResponseModel>();

            // var userFromDatabase = await _context.Users.FirstOrDefaultAsync(userData => userData.Email == loginDTO.Email);

            // if (userFromDatabase == null || !CompareHashPassword(loginDTO.Password, userFromDatabase.HashPassword))
            // {
            //     response.Message = "Email or Password incorrect";
            //     return response;
            // }

            var refreshToken = GenerateRefreshToken(loginDTO.Email);
            TokenResponseModel Tokens = GenerateAccessToken(refreshToken);

            response.Data = Tokens;


            return response;
        }

        public async Task<dynamic> RegistrateUser(User user)
        {
            _context.Users.Add(user);
            return await _context.SaveChangesAsync() > 0 ? user : (dynamic)"Unable to register user";
        }

        public ResponseModel<TokenResponseModel> GenerateAccessTokenWithResponse(string refreshToken)
        {
            ResponseModel<TokenResponseModel> response = new ResponseModel<TokenResponseModel>();

            try
            {
                TokenResponseModel tokens = GenerateAccessToken(refreshToken);
                response.Data = tokens;
                response.Status = true;
                response.Message = "Access token generated successfully.";
            }
            catch (SecurityTokenException ex)
            {
                response.Status = false;
                response.Message = $"Token validation error: {ex.Message}";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return response;
        }

    }
}
