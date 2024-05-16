using DevJobsBackend.Dtos;
using DevJobsBackend.Entities;

namespace DevJobsBackend.Contracts.Services;

public interface IAuthService
{
    Task<string> GenerateHashPassword(string password);
    bool CompareHashPassword(string UserPassword, string DatabasePassword);

    string GenerateJwtToken(string email);
    string RefreshJwtToken (string token);
    Task<ResponseModel<TokenResponseModel>> Login(LoginDTO loginDTO);
    Task<dynamic> RegistrateUser(User user);


}
