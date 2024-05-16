using DevJobsBackend.Entities;

namespace DevJobsBackend.Contracts.Services;

public interface IAuthService
{
    string GenerateHashPassword(string password);
    dynamic RegistrateUser(User user);
    Task<string> ResetPassword(string Email, string NewPassword);
}
