using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CrimeManagementApi.DTOs;
using CrimeManagementApi.Services.Interfaces;

namespace CrimeManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Investigator")]
    public class CaseAssigneeController : ControllerBase
    {
        private readonly ICaseAssigneeService _service;

        public CaseAssigneeController(ICaseAssigneeService service)
        {
            _service = service;
        }

        [HttpGet("case/{caseId}")]
        public async Task<IActionResult> GetByCase(int caseId)
        {
            var list = await _service.GetByCaseIdAsync(caseId);
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Assign([FromBody] CreateCaseAssigneeDto dto)
        {
            var created = await _service.AssignAsync(dto);
            return CreatedAtAction(nameof(GetByCase), new { caseId = dto.CaseId }, created);
        }

        [HttpPut("{id}/progress")]
        [Authorize(Roles = "Admin,Officer,Investigator")]
        public async Task<IActionResult> UpdateProgress(int id, [FromQuery] string status)
        {
            var updated = await _service.UpdateProgressAsync(id, status);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var success = await _service.RemoveAsync(id);
            return success ? Ok("Assignee removed from case.") : NotFound();
        }
    }
}
