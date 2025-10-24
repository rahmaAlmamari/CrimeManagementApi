using CrimeManagementApi.DTOs;
using CrimeManagementApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CrimeManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CaseParticipantController : ControllerBase
    {
        private readonly ICaseParticipantService _service;
        private readonly ILogger<CaseParticipantController> _logger;

        public CaseParticipantController(ICaseParticipantService service, ILogger<CaseParticipantController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Link([FromBody] CreateCaseParticipantDto dto)
        {
            var result = await _service.LinkAsync(dto);

            if (result == null)
                return Conflict(new { message = "Participant already linked or invalid Case/Participant ID." });

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
