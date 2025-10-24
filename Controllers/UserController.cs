using System.Security.Claims;
using CrimeManagementApi.DTOs;
using CrimeManagementApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrimeManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // =============================================================
        // 🔹 ADMIN MANAGEMENT ENDPOINTS
        // =============================================================

        /// <summary>
        /// Get paginated list of users, with optional filters by role or clearance level.
        /// Example: /api/user?role=Officer&page=2&pageSize=10
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? role,
            [FromQuery] string? clearanceLevel,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("Page and pageSize must be greater than 0.");

            var allUsers = await _userService.GetAllAsync();

            // 🔹 Apply filters (if any)
            var filtered = allUsers.AsQueryable();
            if (!string.IsNullOrWhiteSpace(role))
                filtered = filtered.Where(u => u.Role.Equals(role, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(clearanceLevel))
                filtered = filtered.Where(u => u.ClearanceLevel.Equals(clearanceLevel, StringComparison.OrdinalIgnoreCase));

            // 🔹 Pagination
            var totalCount = filtered.Count();
            var users = filtered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // 🔹 Build response
            var response = new
            {
                page,
                pageSize,
                totalUsers = totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                users
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] CreateUserDto dto)
        {
            if (dto == null)
                return BadRequest("User data cannot be empty.");

            var createdUser = await _userService.CreateAsync(dto);
            if (createdUser == null)
                return BadRequest("Failed to create user.");

            _logger.LogInformation("Admin registered new user: {Username} ({Role})", dto.Username, dto.Role);

            return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateUserDto dto)
        {
            var updated = await _userService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _userService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        // =============================================================
        // 🔹 CURRENT USER PROFILE (/me)
        // =============================================================
        /// <summary>
        /// Returns the authenticated user's profile extracted from the JWT.
        /// </summary>
        [HttpGet("Me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(username))
                return Unauthorized("Invalid or missing token.");

            var user = (await _userService.GetAllAsync())
                .FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (user == null)
                return NotFound("User not found.");

            return Ok(new
            {
                user.Username,
                user.Email,
                user.FullName,
                user.Role,
                user.ClearanceLevel
            });
        }
    }
}
