using DevJobsBackend.Entities;
using DevJobsBackend.Responses;
using Microsoft.AspNetCore.Identity;

namespace DevJobsBackend.Contracts.Services
{
    public interface IUserService
    {
        Task<List<User>> GetUsers();
        Task<User> GetUser(int idUser);
        Task<User> GetUserByEmail(string userEmail);
        Task<User> AddUser(User user);
        Task<string> UpdateUser(User user);
        Task<string> RemoveUser(User user);
        Task<User> Me(int IdUser);
    }
}