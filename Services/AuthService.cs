using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using DevJobsBackend.Contracts.Services;
using DevJobsBackend.Data;
using DevJobsBackend.Entities;

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
        public string GenerateHashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
            // string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            // return await Task.FromResult(hashedPassword);
        }
        public dynamic RegistrateUser(User user)
        {
            var hashPassword = GenerateHashPassword(user.HashPassword);
            user.HashPassword = hashPassword;
            _context.Users.Add(user);
            return _context.SaveChanges() > 0
                ? user
                : "Unable to registrate user";
        }
        public async Task<string> ResetPassword(string Email, string NewPassword)
        {
            var user = await _userService.GetUserByEmail(Email) ?? throw new Exception("Unable to find user");
            var newHashedPassword = BCrypt.Net.BCrypt.HashPassword(NewPassword) ?? throw new Exception("Unable to hash password");
            user.HashPassword = newHashedPassword;
            _context.SaveChanges();
            return "Password changed with success";
        }
    }
}
