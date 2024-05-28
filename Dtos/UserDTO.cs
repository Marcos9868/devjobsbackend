using DevJobsBackend.Enums;

namespace DevJobsBackend.Dtos
{
    public class UserDTO
    {
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string HashPassword { get; set; } = string.Empty;
        public UserType TypeUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}