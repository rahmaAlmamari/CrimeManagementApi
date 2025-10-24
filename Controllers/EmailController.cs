using CrimeManagementApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CrimeManagementApi.Controllers
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

        // 🔹 Test general email
        [HttpPost("send")]
        public async Task<IActionResult> SendGeneralEmail(string to)
        {
            await _emailService.SendEmailAsync(to, "Test Email", "<h3>Hello from CrimeManagementApi</h3><p>This is a test email.</p>");
            return Ok(" Email sent successfully.");
        }

        // 🔹 Test case status update
        [HttpPost("case-update")]
        public async Task<IActionResult> SendCaseUpdate(string to, string caseNumber, string status)
        {
            await _emailService.SendCaseUpdateAsync(to, caseNumber, status);
            return Ok(" Case update email sent.");
        }

        // 🔹 Test new crime alert
        [HttpPost("crime-alert")]
        public async Task<IActionResult> SendCrimeAlert(string to, string area, string description)
        {
            await _emailService.SendNewCrimeAlertAsync(to, area, description);
            return Ok(" Crime alert email sent.");
        }
    }
}
