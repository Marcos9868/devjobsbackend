using System.ComponentModel.DataAnnotations;

namespace DevJobsBackend.Dtos
{
    public class ResetPasswordDTO
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string NewPassword { get; set; } = string.Empty;
    }
}