using DevJobsBackend.Dtos;
using DevJobsBackend.Entities;
using DevJobsBackend.Responses;

namespace DevJobsBackend.Contracts.Services;

public interface IAuthService
{
    Task<ResponseBase<TokenResponse>> Login(LoginDTO loginDTO);
    ResponseBase<TokenResponse> GenerateAccessTokenResponse(string refreshToken);
    Task<ResponseBase<User>> RegistrateUser(User user);
    Task<string> ResetPassword(string Email, string NewPassword);
    Task<User> GetUserByAccessToken(string accessToken);

    Task<ResponseBase<object>> SendAccountDeletionConfirmationEmail(User currentUser);


}
