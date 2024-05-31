using System.Collections.Generic;
using System.Threading.Tasks;
using DevJobsBackend.Entities;

namespace DevJobsBackend.Contracts.Services
{
    public interface IEmailService
    {
    
        Task SendEmailAsync(string toEmail, string subject, string htmlContent);
        
        // Métodos para gerenciar templates de e-mail
        Task<IEnumerable<EmailTemplate>> GetAllTemplatesAsync();
        Task<EmailTemplate> GetTemplateByIdAsync(int id);
        Task<EmailTemplate> GetTemplateByNameAsync(string name);
        Task AddTemplateAsync(EmailTemplate template);
        Task UpdateTemplateAsync(EmailTemplate template);
        Task DeleteTemplateAsync(int id);

      
        string ReplacePlaceholders(string templateContent, IDictionary<string, string> placeholders);
    }
}
