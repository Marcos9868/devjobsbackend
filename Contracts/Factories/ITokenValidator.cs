using System.Security.Claims;

namespace DevJobsBackend.Contracts.Factories;


 public interface ITokenValidator
    {
        string ValidateAccessToken(string token, out ClaimsPrincipal claimsPrincipal);
        string ValidateRefreshToken(string token, out ClaimsPrincipal claimsPrincipal);
        string ValidateForgotPasswordToken(string token, out ClaimsPrincipal claimsPrincipal);
        string ValidateDeleteAccountToken(string token, out ClaimsPrincipal claimsPrincipal);
    }
