namespace DevJobsBackend.Entities.VacancyJobs
{
    public class Job : EntityBaseVacancyJob
    {
        public decimal HourExpectation { get; set; } = 0_0;
        public bool EnableChat { get; set; } = true;
        public List<Proposal> Proposals { get; set; } = new List<Proposal>();
    }
}