using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CrimeManagementApi.DTOs;
using CrimeManagementApi.Services.Interfaces;

namespace CrimeManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParticipantController : ControllerBase
    {
        private readonly IParticipantService _participantService;
        private readonly ILogger<ParticipantController> _logger;

        public ParticipantController(IParticipantService participantService, ILogger<ParticipantController> logger)
        {
            _participantService = participantService;
            _logger = logger;
        }

        // ======================================================
        // 🔹 Add Participant (Auto Detect AddedByUser)
        // ======================================================
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateParticipantDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Extract user ID from token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim))
                    dto.AddedByUserId = int.Parse(userIdClaim);

                var participant = await _participantService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = participant.Id }, participant);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding participant");
                return StatusCode(500, new { message = "An error occurred while adding the participant." });
            }
        }

        // ======================================================
        // 🔹 Get Participant by ID
        // ======================================================
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var participant = await _participantService.GetByIdAsync(id);
            if (participant == null)
                return NotFound();

            return Ok(participant);
        }

        // ======================================================
        // 🔹 Get All Participants
        // ======================================================
        [Authorize(Roles = "Admin,Investigator")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var participants = await _participantService.GetAllAsync();
            return Ok(participants);
        }
    }
}
