namespace DevJobsBackend.Contracts.Services;

public interface IAuthService
{
    Task<string> GenerateHashPassword(string password);
    Task<bool> CompareHashPassword(string passwordToCompare);
    Task<ResponseModel<string>> Login(string password);
}
