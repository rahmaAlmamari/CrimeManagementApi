using CrimeManagementApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrimeManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SmsTestController : ControllerBase
    {
        private readonly ISmsService _smsService;

        public SmsTestController(ISmsService smsService)
        {
            _smsService = smsService;
        }

        [HttpPost("test")]
        public async Task<IActionResult> SendTestSms(string phoneNumber)
        {
            try
            {
                await _smsService.SendSmsAsync(phoneNumber, "Test SMS from Crime Management System!");
                return Ok("SMS sent successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to send SMS: {ex.Message}");
            }
        }

        [HttpPost("test-case-update")]
        public async Task<IActionResult> SendTestCaseUpdate(string phoneNumber, string caseNumber = "TEST-001", string status = "In Progress")
        {
            try
            {
                await _smsService.SendCaseUpdateSmsAsync(phoneNumber, caseNumber, status);
                return Ok("Case update SMS sent successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to send case update SMS: {ex.Message}");
            }
        }

        [HttpPost("test-assignment")]
        public async Task<IActionResult> SendTestAssignment(string phoneNumber, string caseNumber = "TEST-001", string assignedTo = "John Doe")
        {
            try
            {
                await _smsService.SendNewCaseAssignmentSmsAsync(phoneNumber, caseNumber, assignedTo);
                return Ok("Assignment SMS sent successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to send assignment SMS: {ex.Message}");
            }
        }
    }
}