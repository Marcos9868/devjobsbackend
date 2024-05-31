using DevJobsBackend.Contracts.Services;
using DevJobsBackend.Entities;
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
        public async Task<IEnumerable<EmailTemplate>> GetAllTemplates()
        {
            return await _emailService.GetAllTemplatesAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTemplateById(int id)
        {
            var template = await _emailService.GetTemplateByIdAsync(id);
            if (template == null) return NotFound();
            return Ok(template);
        }

        [HttpPost]
        public async Task<IActionResult> AddTemplate(EmailTemplate template)
        {
            await _emailService.AddTemplateAsync(template);
            return CreatedAtAction(nameof(GetTemplateById), new { id = template.Id }, template);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTemplate(int id, EmailTemplate template)
        {
            if (id != template.Id) return BadRequest();
            await _emailService.UpdateTemplateAsync(template);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            await _emailService.DeleteTemplateAsync(id);
            return NoContent();
        }
    }
}
