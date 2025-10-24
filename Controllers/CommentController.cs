using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CrimeManagementApi.DTOs;
using CrimeManagementApi.Services.Interfaces;

namespace CrimeManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Investigator,Officer")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _service;

        public CommentController(ICommentService service)
        {
            _service = service;
        }

        // ==========================================================
        // 🔹 Get all comments for a specific case
        // ==========================================================
        [HttpGet("case/{caseId}")]
        public async Task<IActionResult> GetByCase(int caseId)
        {
            var comments = await _service.GetByCaseIdAsync(caseId);
            return Ok(comments);
        }

        // ==========================================================
        // 🔹 Add a new comment
        // ==========================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCommentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetByCase), new { caseId = dto.CaseId }, created);
        }

        // ==========================================================
        // 🔹 Update an existing comment
        // ==========================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCommentDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound($"Comment ID {id} not found.");

            return Ok(updated);
        }

        // ==========================================================
        // 🔹 Soft delete a comment
        // ==========================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted
                ? Ok($"Comment ID {id} deleted.")
                : NotFound($"Comment ID {id} not found.");
        }
    }
}
