namespace DevJobsBackend.Contracts.Services;

public interface IAuthService
{
    Task<String> GenerateHashPassword(string password);

}
