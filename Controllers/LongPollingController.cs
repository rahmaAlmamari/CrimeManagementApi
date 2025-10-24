using CrimeManagementApi.Models;
using CrimeManagementApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrimeManagementApi.Controllers
{
    /// <summary>
    /// Handles long-polling based evidence deletion lifecycle:
    /// initiation, confirmation, and status tracking.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class LongPollingController : ControllerBase
    {
        private readonly ILongPollingService _longPollingService;
        private readonly IAuditService _auditService;
        private readonly ILogger<LongPollingController> _logger;

        public LongPollingController(
            ILongPollingService longPollingService,
            IAuditService auditService,
            ILogger<LongPollingController> logger)
        {
            _longPollingService = longPollingService;
            _auditService = auditService;
            _logger = logger;
        }

        // ============================================================
        // 🔹 1. INITIATE HARD DELETION REQUEST
        // ============================================================
        [HttpPost("initiate/{evidenceId:int}")]
        public async Task<IActionResult> InitiateDeletion(int evidenceId, [FromQuery] int adminUserId)
        {
            try
            {
                string result = await _longPollingService.InitiateDeletionAsync(evidenceId, adminUserId);

                // log the admin’s intent in the audit log
                await _auditService.AddLogAsync(new DTOs.EvidenceAuditDto
                {
                    EvidenceId = evidenceId,
                    ActedByUserId = adminUserId,
                    Action = "INITIATE_DELETE",
                    Details = $"Admin {adminUserId} initiated hard-deletion for evidence {evidenceId}.",
                    ActedAt = DateTime.UtcNow
                });

                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating deletion for evidence {Eid}", evidenceId);
                return StatusCode(500, "Internal server error while initiating deletion.");
            }
        }

        // ============================================================
        // 🔹 2. CONFIRM HARD DELETION
        // ============================================================
        [HttpPost("confirm/{evidenceId:int}")]
        public async Task<IActionResult> ConfirmDeletion(int evidenceId, [FromQuery] int adminUserId, [FromQuery] string confirmation)
        {
            try
            {
                // --- Validate input ---
                if (string.IsNullOrWhiteSpace(confirmation))
                    return BadRequest(new { message = "Confirmation value required (yes/no)." });

                _logger.LogInformation("Admin {AdminId} confirming deletion for Evidence {EvidenceId} with response '{Confirmation}'",
                    adminUserId, evidenceId, confirmation);

                // --- Process deletion ---
                string result = await _longPollingService.ConfirmDeletionAsync(evidenceId, adminUserId, confirmation);

                // --- Add to audit log ---
                try
                {
                    await _auditService.AddLogAsync(new DTOs.EvidenceAuditDto
                    {
                        EvidenceId = evidenceId,
                        ActedByUserId = adminUserId,
                        Action = confirmation.ToLower() == "yes" ? "HARD_DELETE_CONFIRMED" : "HARD_DELETE_CANCELED",
                        Details = result,
                        ActedAt = DateTime.UtcNow
                    });
                }
                catch (Exception logEx)
                {
                    _logger.LogWarning(logEx, "Audit log creation failed for evidence {EvidenceId}", evidenceId);
                }

                // --- Success Response ---
                return Ok(new
                {
                    success = true,
                    message = result,
                    evidenceId,
                    adminUserId,
                    status = confirmation.ToLower() == "yes" ? "Deleted" : "Canceled"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming deletion for Evidence {Eid}", evidenceId);
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }


        // ============================================================
        // 🔹 3. GET DELETION STATUS (LONG POLL)
        // ============================================================
        [HttpGet("status/{evidenceId:int}")]
        [AllowAnonymous] // optional: allow checking without auth
        public async Task<IActionResult> GetDeletionStatus(int evidenceId)
        {
            try
            {
                DeletionStatus status = await _longPollingService.GetDeletionStatusAsync(evidenceId);
                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching deletion status for evidence {Eid}", evidenceId);
                return StatusCode(500, "Internal server error while retrieving status.");
            }
        }
    }
}
