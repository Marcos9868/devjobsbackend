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
using System;
using DevJobsBackend.Responses;

namespace DevJobsBackend.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly DataContext _context;

        public EmailService(IOptions<EmailSettings> emailSettings, DataContext context)
        {
            _emailSettings = emailSettings.Value ?? throw new ArgumentNullException(nameof(emailSettings));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ResponseBase<bool>> SendEmailAsync(string toEmail, string subject, string templateName, IDictionary<string, string>? placeholders)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
            {
                return new ResponseBase<bool> { Status = false, Message = "Recipient email is required." };
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                return new ResponseBase<bool> { Status = false, Message = "Subject is required." };
            }

            // Get the email template by name
            var templateResponse = await GetTemplateByNameAsync(templateName);
            if (templateResponse.Status == false) return new ResponseBase<bool> { Status = false, Message = "Template doesn't exist" };

            var htmlContent = templateResponse.Data.Html;

            if (string.IsNullOrWhiteSpace(htmlContent))
            {
                return new ResponseBase<bool> { Status = false, Message = "Email content is required." };
            }

            // Get the base email template
            var baseTemplateResponse = await GetTemplateByNameAsync("base");
            if (baseTemplateResponse.Status == false) return new ResponseBase<bool> { Status = false, Message = "Base template doesn't exist" };

            var baseHtmlContent = baseTemplateResponse.Data.Html;

            // Replace {content} in the base template with the actual email content
            baseHtmlContent = baseHtmlContent.Replace("{content}", htmlContent);

            if (placeholders != null)
            {
                baseHtmlContent = ReplacePlaceholders(baseHtmlContent, placeholders);
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            message.To.Add(new MailboxAddress(toEmail, toEmail));
            message.Subject = subject;
            if(placeholders != null){
                htmlContent = ReplacePlaceholders(htmlContent,placeholders);
            }
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = baseHtmlContent
            };

            message.Body = bodyBuilder.ToMessageBody();


            
            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPass);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                return new ResponseBase<bool> { Status = true, Data = true, Message = "Email sent successfully." };
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool> { Status = false, Message = $"Error sending email: {ex.Message}" };
            }
        }


        public async Task<ResponseBase<IEnumerable<EmailTemplate>>> GetAllTemplatesAsync()
        {
            var templates = await _context.EmailTemplates.ToListAsync();
            return new ResponseBase<IEnumerable<EmailTemplate>> { Status = true, Data = templates, Message = "Templates retrieved successfully." };
        }

        public async Task<ResponseBase<EmailTemplate>> GetTemplateByIdAsync(int id)
        {
            var template = await _context.EmailTemplates.FindAsync(id);
            if (template == null)
            {
                return new ResponseBase<EmailTemplate> { Status = false, Message = "Template not found." };
            }

            return new ResponseBase<EmailTemplate> { Status = true, Data = template, Message = "Template retrieved successfully." };
        }

        public async Task<ResponseBase<EmailTemplate>> GetTemplateByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new ResponseBase<EmailTemplate> { Status = false, Message = "Template name is required." };
            }

            var template = await _context.EmailTemplates.FirstOrDefaultAsync(t => t.Name == name && t.Active);
            if (template == null)
            {
                return new ResponseBase<EmailTemplate> { Status = false, Message = "Template not found or inactive." };
            }

            return new ResponseBase<EmailTemplate> { Status = true, Data = template, Message = "Template retrieved successfully." };
        }

        public async Task<ResponseBase<bool>> AddTemplateAsync(EmailTemplate template)
        {
            if (template == null)
            {
                return new ResponseBase<bool> { Status = false, Message = "Template cannot be null." };
            }

            _context.EmailTemplates.Add(template);
            await _context.SaveChangesAsync();

            return new ResponseBase<bool> { Status = true, Data = true, Message = "Template added successfully." };
        }

        public async Task<ResponseBase<bool>> UpdateTemplateAsync(EmailTemplate template)
        {
            if (template == null)
            {
                return new ResponseBase<bool> { Status = false, Message = "Template cannot be null." };
            }

            _context.EmailTemplates.Update(template);
            await _context.SaveChangesAsync();

            return new ResponseBase<bool> { Status = true, Data = true, Message = "Template updated successfully." };
        }

        public async Task<ResponseBase<bool>> DeleteTemplateAsync(int id)
        {
            var template = await _context.EmailTemplates.FindAsync(id);
            if (template == null)
            {
                return new ResponseBase<bool> { Status = false, Message = "Template not found." };
            }

            _context.EmailTemplates.Remove(template);
            await _context.SaveChangesAsync();

            return new ResponseBase<bool> { Status = true, Data = true, Message = "Template deleted successfully." };
        }

        private string ReplacePlaceholders(string templateContent, IDictionary<string, string> placeholders)
        {
            if (string.IsNullOrWhiteSpace(templateContent))
            {
                throw new ArgumentException("Template content cannot be null or empty.", nameof(templateContent));
            }

            if (placeholders == null)
            {
                throw new ArgumentNullException(nameof(placeholders));
            }

            foreach (var placeholder in placeholders)
            {
                templateContent = templateContent.Replace($"{{{placeholder.Key}}}", placeholder.Value);
            }
            return templateContent;
        }
    }
}
