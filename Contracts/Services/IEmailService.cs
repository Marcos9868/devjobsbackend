using System.Collections.Generic;
using System.Threading.Tasks;
using DevJobsBackend.Entities;
using DevJobsBackend.Responses;

namespace DevJobsBackend.Contracts.Services
{
    public interface IEmailService
    {
        Task<ResponseBase<bool>> SendEmailAsync(string toEmail, string subject, string templateName,IDictionary<string, string> ?placeholders); 
        Task<ResponseBase<IEnumerable<EmailTemplate>>> GetAllTemplatesAsync();
        Task<ResponseBase<EmailTemplate>> GetTemplateByIdAsync(int id);
        Task<ResponseBase<EmailTemplate>> GetTemplateByNameAsync(string name);
        Task<ResponseBase<bool>> AddTemplateAsync(EmailTemplate template);
        Task<ResponseBase<bool>> UpdateTemplateAsync(EmailTemplate template);
        Task<ResponseBase<bool>> DeleteTemplateAsync(int id); 
    }
}
 
