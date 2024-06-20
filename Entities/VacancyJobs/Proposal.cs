namespace DevJobsBackend.Entities.VacancyJobs
{
    public class Proposal
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<string> Content { get; set; } = new List<string>();
        public string Author { get; set; } = string.Empty;
        public DateTime Created_At { get; set; } = DateTime.Now;
        public decimal ValueJob { get; set; } = 0_0;
    }
}