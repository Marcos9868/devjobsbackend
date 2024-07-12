using DevJobsBackend.Contracts.Factories;
using DevJobsBackend.Contracts.Services;
using DevJobsBackend.Data;
using DevJobsBackend.Dtos;
using DevJobsBackend.Entities;
using DevJobsBackend.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DevJobsBackend.Services
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _context;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly ITokenFactory _tokenFactory;
        private readonly ITokenValidator _tokenValidator;
        private readonly IConfiguration _configuration;

        public AuthService(DataContext context, IUserService userService, IEmailService emailService, ITokenFactory tokenFactory, ITokenValidator tokenValidator, IConfiguration configuration)
        {
            _context = context;
            _userService = userService;
            _emailService = emailService;
            _tokenFactory = tokenFactory;
            _tokenValidator = tokenValidator;
            _configuration = configuration;
        }

        private bool VerifyPasswordHash(string enteredPassword, string storedPasswordHash)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(enteredPassword);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                string hashedPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                return hashedPassword == storedPasswordHash;
            }
        }

        public string GenerateHashPassword(string password)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(password);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                string hashedPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                return hashedPassword;
            }
        }

        public async Task<ResponseBase<TokenResponse>> Login(LoginDTO loginDTO)
        {
            ResponseBase<TokenResponse> response = new ResponseBase<TokenResponse>();

            try
            {
                var userFromDatabase = await _context.Users.FirstOrDefaultAsync(userData => userData.Email == loginDTO.Email);

                if (userFromDatabase == null || !VerifyPasswordHash(loginDTO.Password, userFromDatabase.HashPassword))
                {
                    response.Status = false;
                    response.Message = "Email or Password incorrect";
                    return response;
                }

                var refreshToken = _tokenFactory.CreateRefreshToken(loginDTO.Email);
                var accessToken = _tokenFactory.CreateAccessToken(loginDTO.Email);
                
                TokenResponse tokens = new TokenResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };

                response.Data = tokens;
                response.Status = true;
                response.Message = "Login successful";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return response;
        }

        public async Task<ResponseBase<User>> RegistrateUser(User user)
        {
            user.HashPassword =  GenerateHashPassword(user.HashPassword);
            ResponseBase<User> response = new ResponseBase<User>()
            {
                Data = user,
                Status = true,
                Message = "User registered successfully."
            };

            try
            {
                if (user == null) throw new Exception("Unable to registrate user");
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = $"An unexpected error occured: {ex.Message}";
            }
            return response;
        }

        public ResponseBase<TokenResponse> GenerateAccessTokenResponse(string refreshToken)
        {
            ResponseBase<TokenResponse> response = new ResponseBase<TokenResponse>();

            try
            {
                ClaimsPrincipal claimsPrincipal;
                var email = _tokenValidator.ValidateRefreshToken(refreshToken, out claimsPrincipal);
                var accessToken = _tokenFactory.CreateAccessToken(email);
                var newRefreshToken = _tokenFactory.CreateRefreshToken(email);

                TokenResponse tokens = new TokenResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = newRefreshToken
                };

                response.Data = tokens;
                response.Status = true;
                response.Message = "Access token generated successfully.";
            }
            catch (SecurityTokenException ex)
            {
                response.Status = false;
                response.Message = $"Token validation error: {ex.Message}";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return response;
        }

        public async Task<ResponseBase<string>> ForgotPassword(User user)
        {
            ResponseBase<string> response = new ResponseBase<string>();

            try
            {
                if (user.Email == null) throw new Exception("Unable to registrate user");

                var token = _tokenFactory.CreateForgotPasswordToken(user.Email);

                string clientUrl = _configuration["ExternalUrls:Client_URl"];
                var placeholders = new Dictionary<string, string> {
                    {"name",user.Name},
                    {"reset_link",$"{clientUrl}/resetpassword/{token}"}
                };

                await _emailService.SendEmailAsync(user.Email, "Reset Your Password", "ForgotPassword", placeholders);

                response.Status = true;
                response.Message = "Email enviado com sucesso";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = $"An unexpected error occured: {ex.Message}";
            }

            return response;
        }

        public string ValidateForgotPasswordTokenAndGetEmail(string jwtToken)
        {
            try
            {
                ClaimsPrincipal claimsPrincipal;
                return _tokenValidator.ValidateForgotPasswordToken(jwtToken, out claimsPrincipal);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<User> GetUserByAccessToken(string accessToken)
        {
            try
            {
                ClaimsPrincipal claimsPrincipal;
                var email = _tokenValidator.ValidateAccessToken(accessToken, out claimsPrincipal);

                if (string.IsNullOrEmpty(email))
                {
                    throw new SecurityTokenException("Invalid access token");
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    throw new Exception("User not found");
                }

                return user;
            }
            catch (Exception ex)
            {
                throw new SecurityTokenException("Invalid access token", ex);
            }
        }

        public async Task<ResponseBase<object>> SendAccountDeletionConfirmationEmail(User currentUser)
        {
            try
            {
                var token = _tokenFactory.CreateDeleteAccountToken(currentUser.Email);

                var placeholder = new Dictionary<string, string> {
                    { "confirmation_link", "http://localhost:3000/deleteAccount/"+token },
                    {"name",currentUser.Name}
                };

                await _emailService.SendEmailAsync(currentUser.Email, "Você deseja mesmo deletar sua conta?", "DeleteAccountConfirmation", placeholder);

                return new ResponseBase<object>
                {
                    Status = true,
                    Message = "Email de confirmação de exclusão de conta enviado com sucesso.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseBase<object>
                {
                    Status = false,
                    Message = $"Ocorreu um erro ao enviar o email de confirmação: {ex.Message}",
                    Data = null
                };
            }
        }

        public string ValidateDeleteAccountToken(string deleteAccountToken)
        {
            try
            {
                ClaimsPrincipal claimsPrincipal;
                return _tokenValidator.ValidateDeleteAccountToken(deleteAccountToken, out claimsPrincipal);
            }
            catch (Exception ex)
            {
                throw new SecurityTokenException("Invalid delete account token", ex);
            }
        }
    }
}
