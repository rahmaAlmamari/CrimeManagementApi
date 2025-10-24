using CrimeManagementApi.DTOs;
using CrimeManagementApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrimeManagementApi.Controllers
{
    /// <summary>
    /// Manages evidence records, file uploads, and their audit logs.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EvidenceController : ControllerBase
    {
        private readonly IEvidenceService _evidenceService;
        private readonly IAuditService _auditService;
        private readonly ILogger<EvidenceController> _logger;
        private readonly IWebHostEnvironment _env;

        public EvidenceController(
            IEvidenceService evidenceService,
            IAuditService auditService,
            IWebHostEnvironment env,
            ILogger<EvidenceController> logger)
        {
            _evidenceService = evidenceService;
            _auditService = auditService;
            _env = env;
            _logger = logger;
        }

        // ==========================================================
        // 🔹 CREATE Evidence (JSON)
        // ==========================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEvidenceDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _evidenceService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating evidence");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ==========================================================
        // 🔹 CREATE Evidence (File Upload)
        // ==========================================================
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadEvidence([FromForm] UploadEvidenceDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("No file uploaded.");

            var result = await _evidenceService.UploadAsync(dto);
            if (result == null)
                return BadRequest("Upload failed. Check your input or logs for details.");

            return Ok(result);
        }

        // ==========================================================
        // 🔹 GET Evidence by ID
        // ==========================================================
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var evidence = await _evidenceService.GetByIdAsync(id);
            if (evidence == null)
                return NotFound(new { message = $"Evidence with ID {id} not found." });

            return Ok(evidence);
        }

        // ==========================================================
        // 🔹 GET Evidence by Case ID
        // ==========================================================
        [HttpGet("case/{caseId:int}")]
        public async Task<IActionResult> GetByCaseId(int caseId)
        {
            var evidences = await _evidenceService.GetByCaseIdAsync(caseId);
            return Ok(evidences);
        }

        // ==========================================================
        // 🔹 UPDATE Evidence
        // ==========================================================
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateEvidenceDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _evidenceService.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound(new { message = $"Evidence with ID {id} not found." });

            // Audit log
            await _auditService.AddLogAsync(new EvidenceAuditDto
            {
                EvidenceId = id,
                ActedByUserId = dto.AddedByUserId,
                Action = "UPDATE",
                Details = $"Evidence {id} updated by user {dto.AddedByUserId}.",
                ActedAt = DateTime.UtcNow
            });

            return Ok(updated);
        }

        // ==========================================================
        // 🔹 SOFT DELETE Evidence
        // ==========================================================
        [HttpDelete("{id:int}/soft")]
        public async Task<IActionResult> SoftDelete(int id, [FromQuery] int actedByUserId)
        {
            var deleted = await _evidenceService.SoftDeleteAsync(id, actedByUserId);
            if (!deleted)
                return NotFound(new { message = $"Evidence with ID {id} not found." });

            await _auditService.AddLogAsync(new EvidenceAuditDto
            {
                EvidenceId = id,
                ActedByUserId = actedByUserId,
                Action = "SOFT_DELETE",
                Details = $"Evidence {id} soft-deleted by user {actedByUserId}.",
                ActedAt = DateTime.UtcNow
            });

            return Ok(new { message = $"Evidence {id} soft-deleted successfully." });
        }

        // ==========================================================
        // 🔹 DOWNLOAD Evidence File by ID
        // ==========================================================
        [HttpGet("{id:int}/download")]
        public async Task<IActionResult> Download(int id, [FromQuery] int actedByUserId)
        {
            try
            {
                var evidence = await _evidenceService.GetByIdAsync(id);
                if (evidence == null)
                    return NotFound(new { message = $"Evidence with ID {id} not found." });

                if (string.IsNullOrEmpty(evidence.FilePath))
                    return BadRequest(new { message = "No file is attached to this evidence record." });

                // Get full physical path
                string fullPath = Path.Combine(_env.WebRootPath ?? "wwwroot", evidence.FilePath);
                if (!System.IO.File.Exists(fullPath))
                    return NotFound(new { message = $"File not found on server: {evidence.FilePath}" });

                // Read file bytes
                var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
                string fileName = Path.GetFileName(fullPath);
                string mimeType = evidence.MimeType ?? "application/octet-stream";

                // Log audit entry
                await _auditService.AddLogAsync(new EvidenceAuditDto
                {
                    EvidenceId = id,
                    ActedByUserId = actedByUserId,
                    Action = "DOWNLOAD",
                    Details = $"User {actedByUserId} downloaded evidence file '{fileName}'.",
                    ActedAt = DateTime.UtcNow
                });

                // Return the file for download
                return File(fileBytes, mimeType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading evidence file for ID {Eid}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ==========================================================
        // 🔹 GET All Audit Logs
        // ==========================================================
        [HttpGet("audits")]
        public async Task<IActionResult> GetAllAuditLogs()
        {
            var logs = await _auditService.GetAllAsync();
            return Ok(logs);
        }

        // ==========================================================
        // 🔹 GET Audit Logs by Evidence ID
        // ==========================================================
        [HttpGet("{evidenceId:int}/audits")]
        public async Task<IActionResult> GetAuditByEvidence(int evidenceId)
        {
            var logs = await _auditService.GetByEvidenceIdAsync(evidenceId);
            if (logs == null || !logs.Any())
                return NotFound(new { message = $"No audit logs found for Evidence ID {evidenceId}." });

            return Ok(logs);
        }

        // ==========================================================
        // 🔹 GET Audit Logs by User ID
        // ==========================================================
        [HttpGet("user/{userId:int}/audits")]
        public async Task<IActionResult> GetAuditByUser(int userId)
        {
            var logs = await _auditService.GetByUserIdAsync(userId);
            if (logs == null || !logs.Any())
                return NotFound(new { message = $"No audit logs found for User ID {userId}." });

            return Ok(logs);
        }

        [HttpGet("analyze-text/{caseId}")]
        public async Task<IActionResult> AnalyzeEvidenceText(int caseId)
        {
            var result = await _evidenceService.AnalyzeTextAsync(caseId);
            if (result == null) return NotFound("No text evidence found for this case.");
            return Ok(result);
        }

        [HttpGet("extract-links/{caseId}")]
        public async Task<IActionResult> ExtractLinks(int caseId)
        {
            var result = await _evidenceService.ExtractLinksAsync(caseId);
            return Ok(result);
        }


    }
}
