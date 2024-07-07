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
    public class TokenValidator : ITokenValidator
    {
        private readonly TokenSettings _tokenSettings;

        public TokenValidator(IOptions<TokenSettings> tokenSettings)
        {
            _tokenSettings = tokenSettings.Value;
        }

        public string ValidateAccessToken(string token, out ClaimsPrincipal claimsPrincipal)
        {
            return ValidateToken(token, _tokenSettings.SecretToken, out claimsPrincipal);
        }

        public string ValidateRefreshToken(string token, out ClaimsPrincipal claimsPrincipal)
        {
            return ValidateToken(token, _tokenSettings.RefreshTokenSecret, out claimsPrincipal);
        }

        public string ValidateForgotPasswordToken(string token, out ClaimsPrincipal claimsPrincipal)
        {
            return ValidateToken(token, _tokenSettings.ForgotPasswordSecret, out claimsPrincipal);
        }

        public string ValidateDeleteAccountToken(string token, out ClaimsPrincipal claimsPrincipal)
        {
            return ValidateToken(token, _tokenSettings.DeleteAccountTokenSecret, out claimsPrincipal);
        }

        private string ValidateToken(string token, string secret, out ClaimsPrincipal claimsPrincipal)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secret);

            try
            {
                claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _tokenSettings.Issuer,
                    ValidAudience = _tokenSettings.Audience,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                if (validatedToken.ValidTo < DateTime.UtcNow)
                {
                    throw new SecurityTokenException("Token has expired");
                }

                // Extração do claim do email
                var emailClaim = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(emailClaim))
                {
                    throw new SecurityTokenException("Invalid token: missing email claim");
                }

                return emailClaim;
            }
            catch (Exception)
            {
                claimsPrincipal = null;
                throw new SecurityTokenException("Invalid token");
            }
        }
    }
}
