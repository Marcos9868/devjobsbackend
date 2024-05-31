using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using DevJobsBackend.Configuration;
using DevJobsBackend.Contracts.Services;
using DevJobsBackend.Entities;
using DevJobsBackend.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevJobsBackend.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly DataContext _context;

        public EmailService(IOptions<EmailSettings> emailSettings, DataContext context)
        {
            _emailSettings = emailSettings.Value;
            _context = context;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            message.To.Add(new MailboxAddress(toEmail, toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlContent
            };

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPass);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        // Métodos para gerenciar templates de e-mail
        public async Task<IEnumerable<EmailTemplate>> GetAllTemplatesAsync()
        {
            return await _context.EmailTemplates.ToListAsync();
        }

        public async Task<EmailTemplate> GetTemplateByIdAsync(int id)
        {
            return await _context.EmailTemplates.FindAsync(id);
        }

        public async Task<EmailTemplate> GetTemplateByNameAsync(string name)
        {
            return await _context.EmailTemplates.FirstOrDefaultAsync(t => t.Name == name && t.Active);
        }

        public async Task AddTemplateAsync(EmailTemplate template)
        {
            _context.EmailTemplates.Add(template);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTemplateAsync(EmailTemplate template)
        {
            _context.EmailTemplates.Update(template);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTemplateAsync(int id)
        {
            var template = await _context.EmailTemplates.FindAsync(id);
            if (template != null)
            {
                _context.EmailTemplates.Remove(template);
                await _context.SaveChangesAsync();
            }
        }

        public string ReplacePlaceholders(string templateContent, IDictionary<string, string> placeholders)
        {
            foreach (var placeholder in placeholders)
            {
                templateContent = templateContent.Replace($"{{{placeholder.Key}}}", placeholder.Value);
            }
            return templateContent;
        }
    }
}
