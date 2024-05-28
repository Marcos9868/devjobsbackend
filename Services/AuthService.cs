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
        private readonly IUserService _userService;

        public AuthService(DataContext context, IConfiguration configuration, IUserService userService)
        {
            _context = context;
            _configuration = configuration;
            _userService = userService;

        }

        private bool VerifyPasswordHash(string enteredPassword, string storedPasswordHash)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(enteredPassword);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                string hashedPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                return hashedPassword == storedPasswordHash;
            }
        }

        private Task<string> GenerateHashPassowrd(string password)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(password);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                string hashedPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                return Task.FromResult(hashedPassword);
            }
        }

        private TokenResponseModel GenerateNewTokens(string refreshToken)
        {
            var email = ValidateRefreshToken(refreshToken);
            var accessToken = CreateAccessToken(email);
            var newRefreshToken = CreateRefreshToken(email);
            TokenResponseModel tokens = new TokenResponseModel
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken
            };
            return tokens;
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

        private string CreateAccessToken(string email)
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

        private string CreateRefreshToken(string email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:RefreshTokenSecret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["AppSettings:Issuer"],
                audience: _configuration["AppSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<ResponseModel<TokenResponseModel>> Login(LoginDTO loginDTO)
        {
            ResponseModel<TokenResponseModel> response = new ResponseModel<TokenResponseModel>();

            try
            {
                // var userFromDatabase = await _context.Users.FirstOrDefaultAsync(userData => userData.Email == loginDTO.Email);

                // if (userFromDatabase == null || !VerifyPasswordHash(loginDTO.Password, userFromDatabase.HashPassword))
                // {
                //     response.Status = false;
                //     response.Message = "Email or Password incorrect";
                //     return response;
                // }

                var refreshToken = CreateRefreshToken(loginDTO.Email);
                TokenResponseModel tokens = GenerateNewTokens(refreshToken);

                response.Data = tokens;
                response.Status = true;
                response.Message = "Login successful";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return response;
        }

        public async Task<ResponseModel<User>> RegistrateUser(User user)
        {
            ResponseModel<User> response = new()
            {
                Data = user,
                Status = true,
                Message = "User registered successfully."
            };

            try
            {
                if (user == null) throw new Exception("Unable to registrate user");
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = $"An unexpected error occured: {ex.Message}";
            }
            return response;
        }
        public ResponseModel<TokenResponseModel> GenerateAccessTokenResponse(string refreshToken)
        {
            ResponseModel<TokenResponseModel> response = new ResponseModel<TokenResponseModel>();

            try
            {
                TokenResponseModel tokens = GenerateNewTokens(refreshToken);
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
        public async Task<string> ResetPassword(string Email, string NewPassword)
        {
            var user = await _userService.GetUserByEmail(Email) ?? throw new Exception("Unable to find user");
            var newHashedPassword = await GenerateHashPassowrd(NewPassword) ?? throw new Exception("Unable to hash password");
            user.HashPassword = newHashedPassword;
            _context.SaveChanges();
            return "Password changed with success";
        }
    }
}
