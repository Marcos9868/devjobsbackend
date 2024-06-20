namespace DevJobsBackend.Entities.VacancyJobs
{
    public class EntityBaseVacancyJob
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime Created_At { get; set; } = DateTime.Now;
        public List<string> Description { get; set; } = new List<string>();
        public List<string> Keywords { get; set; } = new List<string>();
    }
}