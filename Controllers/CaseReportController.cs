using CrimeManagementApi.DTOs;
using CrimeManagementApi.Models;
using CrimeManagementApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrimeManagementApi.Controllers
{
    /// <summary>
    /// API Controller for managing the link between Cases and Crime Reports.
    /// Uses composite keys (CaseId + ReportId) as identifiers.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 🔐 All endpoints require JWT authentication unless stated otherwise
    public class CaseReportController : ControllerBase
    {
        private readonly ICaseReportService _service;
        private readonly ILogger<CaseReportController> _logger;

        public CaseReportController(ICaseReportService service, ILogger<CaseReportController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // ==========================================================
        // 🔹 GET: api/casereport
        // ==========================================================
        /// <summary>
        /// Retrieves all case-report links visible to the authenticated user.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _service.GetAllAsync();
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex.Message);
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving case-report links.");
                return StatusCode(500, "Internal server error.");
            }
        }

        // ==========================================================
        // 🔹 GET: api/casereport/{caseId}/{reportId}
        // ==========================================================
        /// <summary>
        /// Retrieves a single case-report link using composite keys.
        /// </summary>
        [HttpGet("{caseId:int}/{reportId:int}")]
        public async Task<IActionResult> GetByIds(int caseId, int reportId)
        {
            try
            {
                var link = await _service.GetByIdsAsync(caseId, reportId);
                if (link == null)
                    return NotFound("Link not found.");

                return Ok(link);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving case-report link.");
                return StatusCode(500, "Internal server error.");
            }
        }

        // ==========================================================
        // 🔹 POST: api/casereport/link
        // ==========================================================
        /// <summary>
        /// Creates a new link between a case and a crime report.
        /// </summary>
        [HttpPost("link")]
        public async Task<IActionResult> Link([FromBody] CreateCaseReportDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.LinkAsync(dto);
                if (result == null)
                    return BadRequest("Link could not be created (possibly already exists).");

                return Ok(new { message = "Link created successfully", result });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex.Message);
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating case-report link.");
                return StatusCode(500, "Internal server error.");
            }
        }

        // ==========================================================
        // 🔹 PUT: api/casereport/{caseId}/{reportId}
        // ==========================================================
        /// <summary>
        /// Updates an existing case-report link (e.g., notes or reassignment).
        /// </summary>
        [HttpPut("{caseId:int}/{reportId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int caseId, int reportId, [FromBody] UpdateCaseReportDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _service.UpdateAsync(caseId, reportId, dto);
                if (updated == null)
                    return NotFound("Link not found for update.");

                return Ok(new { message = "Link updated successfully", updated });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating case-report link.");
                return StatusCode(500, "Internal server error.");
            }
        }

        // ==========================================================
        // 🔹 DELETE: api/casereport/{caseId}/{reportId}
        // ==========================================================
        /// <summary>
        /// Unlinks (removes) the relationship between a case and a report.
        /// </summary>
        [HttpDelete("{caseId:int}/{reportId:int}")]
        public async Task<IActionResult> Unlink(int caseId, int reportId)
        {
            try
            {
                var success = await _service.UnlinkAsync(caseId, reportId);
                if (!success)
                    return NotFound("No link found to remove.");

                return Ok(new { message = "Link removed successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unlinking case-report.");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
