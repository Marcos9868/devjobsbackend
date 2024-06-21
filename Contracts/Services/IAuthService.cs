using DevJobsBackend.Dtos;
using DevJobsBackend.Entities;
using DevJobsBackend.Responses;

namespace DevJobsBackend.Contracts.Services;

public interface IAuthService
{
    Task<ResponseBase<TokenResponse>> Login(LoginDTO loginDTO);
    ResponseBase<TokenResponse> GenerateAccessTokenResponse(string refreshToken);
    Task<ResponseBase<User>> RegistrateUser(User user);
    Task<ResponseBase<string>> ForgotPassword(string Email);
    string GenerateHashPassword(string password);
    string ValidateForgotPasswordTokenAndGetEmail(string jwtToken);
    Task<User> GetUserByAccessToken(string accessToken);
    Task<ResponseBase<object>> SendAccountDeletionConfirmationEmail(User currentUser);

    string ValidateDeleteAccountToken(string DeleteAccountToken);



}
