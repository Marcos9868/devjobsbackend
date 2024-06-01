using DevJobsBackend.Dtos;
using DevJobsBackend.Entities;

namespace DevJobsBackend.Contracts.Services;

public interface IAuthService
{    Task<ResponseModel<TokenResponseModel>> Login(LoginDTO loginDTO);
    
    ResponseModel<TokenResponseModel> GenerateAccessTokenResponse(string refreshToken);
    Task<ResponseModel<User>> RegistrateUser(User user);

    Task<string> ResetPassword(string Email, string NewPassword);

Task<User> GetUserByAccessToken(string accessToken);

}
