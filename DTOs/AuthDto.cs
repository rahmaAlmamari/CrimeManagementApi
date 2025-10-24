using System.ComponentModel.DataAnnotations;

namespace CrimeManagementApi.DTOs
{
    // ============================================================
    // 🔹 AuthDtos.cs — Handles authentication-related DTOs
    // ============================================================

    /// <summary>
    /// Represents the data required for user authentication.
    /// Used during login requests to obtain a JWT token.
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Username or email of the user attempting to log in.
        /// </summary>
        [Required(ErrorMessage = "Username or Email is required.")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Plain-text password for authentication.
        /// This value is never stored or logged in plaintext.
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents the authentication response returned after successful login.
    /// Includes a JWT token and optional refresh token.
    /// </summary>
    public class AuthResponseDto
    {
        /// <summary>
        /// JWT access token used for authenticated requests.
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Email associated with the authenticated account.
        /// </summary>
        [Required(ErrorMessage = "Username or Email is required.")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Username associated with the authenticated account.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Role assigned to the authenticated user (e.g., Admin, Officer, Investigator).
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// UTC expiration time of the JWT token.
        /// </summary>
        public DateTime Expiration { get; set; }
    }
}
