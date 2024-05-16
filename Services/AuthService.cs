using System;
using System.Text;
using System.Threading.Tasks;
using DevJobsBackend.Contracts.Services;
using DevJobsBackend.Data;
using DevJobsBackend.Entities;
using System.Security.Cryptography;

namespace DevJobsBackend.Services
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _context;
        private readonly IUserService _userService;

        public AuthService(DataContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public Task<bool> CompareHashPassword(string UserPassword, string DatabasePassword)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(UserPassword);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                string hashedUserPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                return Task.FromResult(hashedUserPassword == DatabasePassword);
            }
        }

        public Task<string> GenerateHashPassword(string password)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(password);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                string hashedPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                return Task.FromResult(hashedPassword);
            }
        }

        public Task<ResponseModel<string>> Login(string password)
        {
            throw new NotImplementedException();
        }

        public async Task<dynamic> RegistrateUser(User user)
        {
            var hashPassword = await GenerateHashPassword(user.HashPassword);
            user.HashPassword = hashPassword;
            _context.Users.Add(user);

            return await _context.SaveChangesAsync() > 0
                ? (dynamic)user
                : null;
        }

        public async Task<string> ResetPassword(string Email, string NewPassword)
        {
            var user = await _userService.GetUserByEmail(Email) ?? throw new Exception("Unable to find user");
            var newHashedPassword = await GenerateHashPassword(NewPassword) ?? throw new Exception("Unable to hash password");
            user.HashPassword = newHashedPassword;
            _context.SaveChanges();
            return "Password changed with success";
        }
    }
}
