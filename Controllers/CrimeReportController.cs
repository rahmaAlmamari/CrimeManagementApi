using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CrimeManagementApi.DTOs;
using CrimeManagementApi.Services.Interfaces;
using System.Security.Claims;

namespace CrimeManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CrimeReportController : ControllerBase
    {
        private readonly ICrimeReportService _service;
        private readonly ILogger<CrimeReportController> _logger;

        public CrimeReportController(ICrimeReportService service, ILogger<CrimeReportController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // ======================================================
        // 🔹 Public: Citizen report submission (no authentication)
        // ======================================================
        [HttpPost("public")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateReport([FromBody] CreateCrimeReportDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var report = await _service.CreateReportAsync(dto);
            return Ok(new { ReportId = report?.Id, Message = "Report submitted successfully." });
        }

        // ======================================================
        // 🔹 Authenticated: Logged-in officer/admin can file reports
        // ======================================================
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateByUser([FromBody] CreateCrimeReportDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // ✅ Extract user ID from JWT token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim))
                    dto.ReportedByUserId = int.Parse(userIdClaim);

                var report = await _service.CreateReportAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = report.Id }, report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating report with user context.");
                return StatusCode(500, new { message = "Error creating report with user context." });
            }
        }

        // ======================================================
        // 🔹 Public: Check status by report ID
        // ======================================================
        [HttpGet("status/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStatus(int id)
        {
            var status = await _service.GetStatusAsync(id);
            return Ok(new { ReportId = id, Status = status });
        }

        // ======================================================
        // 🔹 Admin: List all reports
        // ======================================================
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var reports = await _service.GetAllAsync();
            return Ok(reports);
        }

        // ======================================================
        // 🔹 Admin: View single report by ID
        // ======================================================
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var report = await _service.GetByIdAsync(id);
            return report == null ? NotFound() : Ok(report);
        }
    }
}
