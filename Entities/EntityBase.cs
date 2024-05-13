using DevJobsBackend.Enums;

namespace DevJobsBackend.Entities
{
    public abstract class EntityBase
    {
        public int Id { get; set; }
        public UserType TypeUser { get; set; }
        public string Linkedin { get; set; } = string.Empty;
        public string Instagram { get; set; } = string.Empty;
        public List<string> AboutMe { get; set; } = new List<string>();
        public List<string> Skills { get; set; } = new List<string>();
        public decimal SalaryExpectations { get; set; } = 0_0;
        public decimal HourExpectation { get; set; } = 0_0;
        public bool LegalEntity { get; set; } = false;
        public string Phone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}