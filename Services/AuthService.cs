using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using CrimeManagementApi.Data;
using CrimeManagementApi.DTOs;
using CrimeManagementApi.Services.Interfaces;

namespace CrimeManagementApi.Services
{
    /// <summary>
    /// Handles user authentication and JWT token generation.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthService> _logger;

        public AuthService(AppDbContext context, IConfiguration config, ILogger<AuthService> logger)
        {
            _context = context;
            _config = config;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates a user by verifying email & password, then returns a JWT.
        /// </summary>
        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            try
            {
                // Normalize input
                var email = (dto.Email ?? string.Empty).Trim().ToLowerInvariant();
                var password = dto.Password ?? string.Empty;

                _logger.LogInformation("Login attempt for {Email}", email);

                // Find user (case-insensitive)
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == email);

                if (user == null)
                {
                    _logger.LogWarning("Login failed: user not found for {Email}", email);
                    return null;
                }

                // Verify password with BCrypt
                var passwordOk = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
                if (!passwordOk)
                {
                    _logger.LogWarning("Login failed: invalid password for {Email}", email);
                    return null;
                }

                // Validate JWT settings
                var jwtSection = _config.GetSection("Jwt");
                var keyValue = jwtSection.GetValue<string>("Key");
                if (string.IsNullOrWhiteSpace(keyValue))
                {
                    _logger.LogError("JWT Key missing from configuration.");
                    throw new InvalidOperationException("JWT Key is not configured.");
                }

                var issuer = jwtSection.GetValue<string>("Issuer");
                var audience = jwtSection.GetValue<string>("Audience");
                var expireMinutes = jwtSection.GetValue<double?>("ExpireMinutes") ?? 120;

                // Build claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role ?? string.Empty),
                    new Claim("ClearanceLevel", user.ClearanceLevel.ToString())
                    };
                                                                                

                // Create token
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyValue));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                    signingCredentials: creds
                );

                var jwt = new JwtSecurityTokenHandler().WriteToken(token);

                _logger.LogInformation("User {Email} logged in successfully.", user.Email);

                return new AuthResponseDto
                {
                    Token = jwt,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    Expiration = token.ValidTo
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login for {Email}", dto.Email);
                return null;
            }
        }


    }
}
