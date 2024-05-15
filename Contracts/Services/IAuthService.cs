using DevJobsBackend.Entities;

namespace DevJobsBackend.Contracts.Services;

public interface IAuthService
{
    Task<string> GenerateHashPassword(string password);
    Task<bool> CompareHashPassword(string UserPassword, string DatabasePassword);

    Task<string> GenerateJwtToken(string password);
    Task<string> GenerateJwtRefreshToken (string password);
    Task<ResponseModel<TokenResponseModel>> Login(string password);
    Task<dynamic> RegistrateUser(User user);


}
