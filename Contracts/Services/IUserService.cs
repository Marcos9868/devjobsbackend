using DevJobsBackend.Entities;

namespace DevJobsBackend.Contracts.Services
{
    public interface IUserService
    {
        Task<List<User>> GetUsers();
        Task<User> GetUser(int idUser);
        Task<User> AddUser(User user);
        Task<string> UpdateUser(User user);
        Task<string> RemoveUser(User user);
    }
}