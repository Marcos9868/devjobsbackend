using BCrypt.Net;
using DevJobsBackend.Contracts.Services;
using DevJobsBackend.Data;
﻿
namespace DevJobsBackend.Services{
    public class AuthService : IAuthService
    {
        private readonly DataContext _context;
        
        public AuthService(DataContext context)
        {
            _context = context;
        }

        public Task<string> GenerateHashPassword(string password)
        {
            string hashedPassword = BCrypt.HashPassword(password);
            System.Diagnostics.Debug.WriteLine(hashedPassword);
            return Task.FromResult(hashedPassword);
        }
    }
}
