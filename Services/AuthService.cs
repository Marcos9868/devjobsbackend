using DevJobsBackend.Contracts.Services;
using DevJobsBackend.Data;
using DevJobsBackend.Entities;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DevJobsBackend.Services
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _context;

        public AuthService(DataContext context)
        {
            _context = context;
            
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

        public Task<string> GenerateJwtRefreshToken(string password)
        {
            throw new NotImplementedException();
        }

        public Task<string> GenerateJwtToken(string password)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<TokenResponseModel>> Login(string password)
        {
            ResponseModel<TokenResponseModel> response = new ResponseModel<TokenResponseModel>();

            
            throw new NotImplementedException();
        }

        public async Task<dynamic> RegistrateUser(User user)
        {
            _context.Users.Add(user);
            return await _context.SaveChangesAsync() > 0
                ? user
                : (dynamic)"Unable to register user";
        }

    }
}
