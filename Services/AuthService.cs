using DevJobsBackend.Contracts.Services;
using DevJobsBackend.Data;
using DevJobsBackend.Entities;

namespace DevJobsBackend.Services
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _context;

        public AuthService(DataContext context)
        {
            _context = context;
        }
        public async Task<string> GenerateHashPassword(string password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            return await Task.FromResult(hashedPassword);
        }
        public dynamic RegistrateUser(User user)
        {
            _context.Users.Add(user);
            return _context.SaveChanges() > 0 
                ? user 
                : "Unable to registrate user";
        }
    }
}
