using DevJobsBackend.Entities;

namespace DevJobsBackend.Contracts.Services;

public interface IAuthService
{
    Task<string> ResetPassword(string Email, string NewPassword);
    Task<string> GenerateHashPassword(string password);
    Task<bool> CompareHashPassword(string UserPassword, string DatabasePassword);
    Task<ResponseModel<string>> Login(string password);
    Task<dynamic> RegistrateUser(User user);
}
