namespace DevJobsBackend.Contracts.Services;

public interface IAuthService
{
    Task<string> GenerateHashPassword(string password);
    Task<bool> CompareHashPassword(string UserPassword, string DatabasePassword);
    Task<ResponseModel<string>> Login(string password);
}
