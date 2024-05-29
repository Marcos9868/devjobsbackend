using DevJobsBackend.Enums;

namespace DevJobsBackend.Entities.VacancyJobs
{
    public class Vacancy : EntityBaseVacancyJob
    {
        public decimal SalaryExpectation { get; set; } = 0_0;
        public List<string> MinimumRequisites { get; set; } = new List<string>();
        public List<string> Benefits { get; set; } = new List<string>();
        public CLT_PJ CLT_PJ { get; set; } = CLT_PJ.NONE;
        public string Location { get; set; } = string.Empty;
    }
}