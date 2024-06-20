using DevJobsBackend.Contracts.Services;
using DevJobsBackend.Entities;
using DevJobsBackend.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevJobsBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpGet]
        [Admin]
        public async Task<ResponseBase<IEnumerable<EmailTemplate>>> GetAllTemplates()
        {
            return await _emailService.GetAllTemplatesAsync();
        }
        [Admin]
        [HttpGet("GetTemplateById/{id}")]
        public async Task<ResponseBase<EmailTemplate>> GetTemplateById(int id)
        {
            return await _emailService.GetTemplateByIdAsync(id);
        }

        [HttpPost("AddTemplate")]
        [Admin]

        public async Task<ResponseBase<bool>> AddTemplate(EmailTemplate template)
        {
            return await _emailService.AddTemplateAsync(template);
        }

        [HttpPut("UpdateTemplate/{id}")]
        [Admin]

        public async Task<ResponseBase<bool>> UpdateTemplate(int id, EmailTemplate template)
        {
            if (id != template.Id) 
                return new ResponseBase<bool> { Status = false, Message = "Template ID mismatch." };

            return await _emailService.UpdateTemplateAsync(template);
        }

        [HttpDelete("DeleteTemplate/{id}")]
        [Admin]

        public async Task<ResponseBase<bool>> DeleteTemplate(int id)
        {
            return await _emailService.DeleteTemplateAsync(id);
        }
    }
}
