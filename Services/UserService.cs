using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using DevJobsBackend.Contracts.Services;
using DevJobsBackend.Data;
using DevJobsBackend.Dtos;
using DevJobsBackend.Entities;
using DevJobsBackend.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DevJobsBackend.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;

        private readonly Func<IAuthService> _authServiceFactory;
        
        public UserService(DataContext context, Func<IAuthService> authServiceFactory)
        {
            _context = context;
            _authServiceFactory = authServiceFactory;
        }

        private IAuthService AuthService => _authServiceFactory();

        public async Task<User> AddUser(User user)
        {
            User newUser = new()
            {
                Name = user.Name,
                LastName = user.LastName,
                Email = user.Email,
                HashPassword = user.HashPassword
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return newUser;
        }

        public async Task<List<User>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users ?? new List<User>();
        }

        public async Task<User> GetUser(int idUser)
        {
            var user = await _context.Users.FindAsync(idUser) ?? throw new Exception("Unable to find user");

            return user;
        }

        public async Task<User> GetUserByEmail(string userEmail)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail) ??
            throw new Exception("Unable to find user");
            return user;
        }

        public async Task<string> UpdateUser(User user)
        {
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0 ? "User updated" : "Unable to update user";


        }
        public async Task<ResponseBase<object>> RemoveUser(DeleteAccountTokenDTO DeleteAccountTokenDTO)
        {
         try{

                var UserEmail = AuthService.ValidateDeleteAccountToken(DeleteAccountTokenDTO.DeleteAccountToken);

                var user = await GetUserByEmail(UserEmail);

                _context.Users.Remove(user);
                
                await _context.SaveChangesAsync();

                return new ResponseBase<object>
                {
                    Status = true,
                    Message = "Usuario removido com sucesso!",
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

        public async Task<User> Me(int IdUser)
        {
            var user = await _context.Users.FindAsync(IdUser) ?? throw new Exception("Unable to find user");
            return user;
        }

        public async Task<ResponseBase<User>> ResetPassword(string newPassword, string JwtToken)
        {
            var response = new ResponseBase<User>();

            try
            {
                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    throw new ArgumentException("New password cannot be empty");
                }

                var email = AuthService.ValidateForgotPasswordTokenAndGetEmail(JwtToken);

                if (email == null)
                {
                    throw new SecurityTokenException("Invalid token");
                }

                var user = await GetUserByEmail(email);
                if (user == null)
                {
                    throw new Exception("User not found");
                }

                user.HashPassword = AuthService.GenerateHashPassword(newPassword);
                await UpdateUser(user);

                response.Data = user;
                response.Status = true;
                response.Message = "Password reset successfully";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return response;
        }
    }
}
