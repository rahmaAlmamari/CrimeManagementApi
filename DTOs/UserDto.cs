using System.ComponentModel.DataAnnotations;

namespace CrimeManagementApi.DTOs
{
    // ============================================================
    // 🔹 UserDto.cs — Safe data structure for API user responses
    // ============================================================

    /// <summary>
    /// Represents a sanitized version of user data returned to clients.
    /// Prevents exposure of sensitive information like passwords or tokens.
    /// </summary>
    public class UserDto
    {
        /// <summary>Unique identifier of the user.</summary>
        public int Id { get; set; }

        /// <summary>Unique username used for authentication or reference.</summary>
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        /// <summary>Full legal name of the user.</summary>
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        /// <summary>Email address of the user.</summary>
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>Assigned role (Admin, Officer, Investigator, Citizen).</summary>
        [Required]
        public string Role { get; set; } = string.Empty;

        /// <summary>Security clearance level (Low, Medium, High, Critical).</summary>
        [Required]
        public string ClearanceLevel { get; set; } = string.Empty;

        /// <summary>Date and time when the account was created (UTC).</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ============================================================
    // 🔹 CreateUserDto.cs — Used for new user registration
    // ============================================================

    /// <summary>
    /// Used when creating or registering a new user.
    /// Carries user credentials and role data from the client to the API.
    /// </summary>
    public class CreateUserDto
    {
        /// <summary>Unique username for login and system reference.</summary>
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        /// <summary>Full legal name of the user.</summary>
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        /// <summary>Valid email address for user communication.</summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>Plain-text password (will be hashed before saving).</summary>
        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; } = string.Empty;

        /// <summary>User role (default: Citizen).</summary>
        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "Citizen";

        /// <summary>Security clearance level (default: Low).</summary>
        [Required]
        [MaxLength(50)]
        public string ClearanceLevel { get; set; } = "Low";
    }
}
