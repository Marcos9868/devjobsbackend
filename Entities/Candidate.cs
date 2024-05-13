namespace DevJobsBackend.Entities
{
    public class Candidate : User
    {
        public byte CoverLetter { get; set; }
        public int RegistrationsMade { get; set; } = 0;
    }
}