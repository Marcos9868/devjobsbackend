namespace DevJobsBackend.Entities
{
    public class Freelancer : User
    {
        public string Portfolio { get; set; } = string.Empty;
        public int WorksDone { get; set; } = 0;
        public int ProposalMade { get; set; } = 0;
    }
}