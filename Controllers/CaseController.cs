using CrimeManagementApi.DTOs;
using CrimeManagementApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CrimeManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Investigator,Officer")]
    public class CaseController : ControllerBase
    {
        private readonly ICaseService _caseService;
        private readonly ILogger<CaseController> _logger;

        public CaseController(ICaseService caseService, ILogger<CaseController> logger)
        {
            _caseService = caseService;
            _logger = logger;
        }

        // ======================================================
        // 🔹 Get All Cases
        // ======================================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cases = await _caseService.GetAllAsync();
            return Ok(cases);
        }

        // ======================================================
        // 🔹 Get Case by ID
        // ======================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _caseService.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        // ======================================================
        // 🔹 Create Case (Auto-detect user from JWT)
        // ======================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCaseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // ✅ Extract user ID from JWT
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var userId))
                    dto.CreatedByUserId = userId;

                var created = await _caseService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating case");
                return StatusCode(500, new { message = "An error occurred while creating the case." });
            }
        }

        // ======================================================
        // 🔹 Update Case (Uses UpdateCaseDto)
        // ======================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCaseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _caseService.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        // ======================================================
        // 🔹 Delete Case
        // ======================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _caseService.DeleteAsync(id);
            return deleted ? Ok(new { message = "Case deleted successfully." }) : NotFound();
        }

        // ======================================================
        // 🔹 Search Cases by Keyword
        // ======================================================
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? keyword)
        {
            var results = await _caseService.SearchAsync(keyword);
            return Ok(results);
        }

        // 🔹 Get All Assignees
        [HttpGet("{caseId:int}/assignees")]
        public async Task<IActionResult> GetAssignees(int caseId)
        {
            var assignees = await _caseService.GetAssigneesByCaseIdAsync(caseId);
            return Ok(assignees);
        }

        // 🔹 Get All Evidence
        [HttpGet("{caseId:int}/evidence")]
        public async Task<IActionResult> GetEvidence(int caseId)
        {
            var evidence = await _caseService.GetEvidenceByCaseIdAsync(caseId);
            return Ok(evidence);
        }

        // 🔹 Get All Suspects
        [HttpGet("{caseId:int}/suspects")]
        public async Task<IActionResult> GetSuspects(int caseId)
        {
            var suspects = await _caseService.GetSuspectsByCaseIdAsync(caseId);
            return Ok(suspects);
        }

        // 🔹 Get All Victims
        [HttpGet("{caseId:int}/victims")]
        public async Task<IActionResult> GetVictims(int caseId)
        {
            var victims = await _caseService.GetVictimsByCaseIdAsync(caseId);
            return Ok(victims);
        }

        // 🔹 Get All Witnesses
        [HttpGet("{caseId:int}/witnesses")]
        public async Task<IActionResult> GetWitnesses(int caseId)
        {
            var witnesses = await _caseService.GetWitnessesByCaseIdAsync(caseId);
            return Ok(witnesses);
        }

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetCaseDetails(int id)
        {
            var result = await _caseService.GetCaseDetailsAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
