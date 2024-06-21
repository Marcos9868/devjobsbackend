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

namespace DevJobsBackend.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IAuthService _authService;
        public UserService(DataContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;

        }
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
            return users ?? [];
        }
        public async Task<User> GetUser(int idUser)
        {
            var user = await _context.Users.FindAsync(idUser) ??
            throw new Exception("Unable to find user");
            return user;
        }
        public async Task<User> GetUserByEmail(string userEmail)
        {
            var user = await _context.Users.FindAsync(userEmail) ??
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

                var UserEmail = _authService.ValidateDeleteAccountToken(DeleteAccountTokenDTO.DeleteAccountToken);

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
    }
}