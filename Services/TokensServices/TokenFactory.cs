using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using DevJobsBackend.Configurations;
using DevJobsBackend.Contracts;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DevJobsBackend.Contracts.Factories;

namespace DevJobsBackend.Services
{
    public class TokenFactory : ITokenFactory
    {
        private readonly TokenSettings _tokenSettings;

        public TokenFactory(IOptions<TokenSettings> tokenSettings)
        {
            _tokenSettings = tokenSettings.Value;
        }

        public string CreateAccessToken(string email)
        {
            return CreateToken(email, _tokenSettings.SecretToken, 30);
        }

        public string CreateRefreshToken(string email)
        {
            return CreateToken(email, _tokenSettings.RefreshTokenSecret, 10080); // 7 dias
        }

        public string CreateForgotPasswordToken(string email)
        {
            return CreateToken(email, _tokenSettings.ForgotPasswordSecret, 5);
        }

        public string CreateDeleteAccountToken(string email)
        {
            return CreateToken(email, _tokenSettings.DeleteAccountTokenSecret, 5);
        }

        private string CreateToken(string email, string secret, int expirationMinutes)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email)
            };

            var token = new JwtSecurityToken(
                issuer: _tokenSettings.Issuer,
                audience: _tokenSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
