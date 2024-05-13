namespace DevJobsBackend.Entities;

public interface IAuthService
{
    Task<String> generateHashPassword(string password);

}
