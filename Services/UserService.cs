using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using DevJobsBackend.Contracts.Services;
using DevJobsBackend.Data;
using DevJobsBackend.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DevJobsBackend.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        public UserService(DataContext context)
        {
            _context = context;
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
        public async Task<string> RemoveUser(User user)
        {
            _context.Users.Remove(user);
            return _context.SaveChanges() > 0 ? "User removed" : "Unable to remove user";
        }
        public async Task<User> Me(int IdUser)
        {
            var user = await _context.Users.FindAsync(IdUser) ?? throw new Exception("Unable to find user");
            return user;

        }
    }
}