using System;

namespace DevJobsBackend.Entities
{
    public class EmailTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Html { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool Active { get; set; }
    }
}
