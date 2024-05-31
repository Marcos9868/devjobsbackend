using DevJobsBackend.Dtos;
using DevJobsBackend.Entities;
using DevJobsBackend.Responses;

namespace DevJobsBackend.Contracts.Services;

public interface IAuthService
{    Task<ResponseBase<TokenResponse>> Login(LoginDTO loginDTO);
    
    ResponseBase<TokenResponse> GenerateAccessTokenResponse(string refreshToken);
    Task<ResponseBase<User>> RegistrateUser(User user);

    Task<ResponseBase<string>> ForgotPassword(string Email);


}
