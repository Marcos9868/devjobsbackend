using DevJobsBackend.Dtos;
using DevJobsBackend.Entities;

namespace DevJobsBackend.Contracts.Services;

public interface IAuthService
{
    Task<string> GenerateHashPassword(string password);

    string GenerateJwtToken(string email);
    string GenerateRefreshToken (string token);
    Task<ResponseModel<TokenResponseModel>> Login(LoginDTO loginDTO);
    Task<dynamic> RegistrateUser(User user);


}
