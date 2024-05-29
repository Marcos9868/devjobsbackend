namespace DevJobsBackend;

public interface IEmailService
{    Task SendEmailAsync(string toEmail, string subject, string htmlContent);
}
