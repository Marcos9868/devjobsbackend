﻿using DevJobsBackend.Contracts.Services;
using DevJobsBackend.Data;
using DevJobsBackend.Dtos;
using DevJobsBackend.Entities;
using DevJobsBackend.Responses;
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
        private readonly IEmailService _emailService;

        public AuthService(DataContext context, IConfiguration configuration, IUserService userService, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _userService = userService;
            _emailService = emailService;

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

        public string GenerateHashPassword(string password)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(password);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                string hashedPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                return hashedPassword;
            }
        }

        private TokenResponse GenerateNewTokens(string refreshToken)
        {
            var email = ValidateRefreshToken(refreshToken);
            var accessToken = CreateAccessToken(email);
            var newRefreshToken = CreateRefreshToken(email);
            TokenResponse tokens = new TokenResponse
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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTTokenSettings:RefreshTokenSecret"])),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _configuration["JWTTokenSettings:Issuer"],
                ValidAudience = _configuration["JWTTokenSettings:Audience"],
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
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTTokenSettings:SecretToken"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JWTTokenSettings:Issuer"],
                audience: _configuration["JWTTokenSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string CreateRefreshToken(string email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTTokenSettings:RefreshTokenSecret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JWTTokenSettings:Issuer"],
                audience: _configuration["JWTTokenSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<ResponseBase<TokenResponse>> Login(LoginDTO loginDTO)
        {
            ResponseBase<TokenResponse> response = new ResponseBase<TokenResponse>();

            try
            {
                var userFromDatabase = await _context.Users.FirstOrDefaultAsync(userData => userData.Email == loginDTO.Email);

                if (userFromDatabase == null || !VerifyPasswordHash(loginDTO.Password, userFromDatabase.HashPassword))
                {
                    response.Status = false;
                    response.Message = "Email or Password incorrect";
                    return response;
                }

                var refreshToken = CreateRefreshToken(loginDTO.Email);
                TokenResponse tokens = GenerateNewTokens(refreshToken);

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

        public async Task<ResponseBase<User>> RegistrateUser(User user)
        {
            user.HashPassword =  GenerateHashPassword(user.HashPassword);
            ResponseBase<User> response = new ResponseBase<User>()
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
        public ResponseBase<TokenResponse> GenerateAccessTokenResponse(string refreshToken)
        {
            ResponseBase<TokenResponse> response = new ResponseBase<TokenResponse>();

            try
            {
                TokenResponse tokens = GenerateNewTokens(refreshToken);
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

        public async Task<ResponseBase<string>> ForgotPassword(string email)
        {
            ResponseBase<string> response = new ResponseBase<string>();

            try
            {
                if (email == null) throw new Exception("Unable to registrate user");

                var user = await _userService.GetUserByEmail(email);

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTTokenSettings:ForgotPasswordSecret"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    new Claim(JwtRegisteredClaimNames.Email, email)
};

                var tokenConfig = new JwtSecurityToken(
                    issuer: _configuration["JWTTokenSettings:Issuer"],
                    audience: _configuration["JWTTokenSettings:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: credentials
                );

                var token = new JwtSecurityTokenHandler().WriteToken(tokenConfig);


                string clientUrl = _configuration["ExternalUrls:Client_URl"];

                var placeholders = new Dictionary<string, string> {
                {"name",user.Name},
                {"reset_link",$"{clientUrl}/resetpassword/{token}"}
            };



                await _emailService.SendEmailAsync(email, "Reset Your Password", "ForgotPassword", placeholders);

                response.Status = true;
                response.Message = "Email enviado com sucesso";

            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = $"An unexpected error occured: {ex.Message}";
            }
            return response;
        }
        public string ValidateForgotPasswordTokenAndGetEmail(string jwtToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JWTTokenSettings:ForgotPasswordSecret"]);

            try
            {
                tokenHandler.ValidateToken(jwtToken, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["JWTTokenSettings:Issuer"],
                    ValidAudience = _configuration["JWTTokenSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                var jwtTokenObj = (JwtSecurityToken)validatedToken;
                var emailClaim = jwtTokenObj.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email);

                return emailClaim?.Value;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return null;
            }

        }
    }
}
