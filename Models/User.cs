using CrimeManagementApi.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrimeManagementApi.Models
{
    /// <summary>
    /// Represents a system user within the Crime Management System.
    /// Each user has a defined role (Admin, Investigator, Officer, or Citizen)
    /// and a clearance level that controls access to sensitive data.
    /// </summary>
    [Table("users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Unique username used for authentication.
        /// </summary>
        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Email address for the user (used for login, notifications, and verification).
        /// </summary>
        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Securely hashed password (never store plaintext passwords).
        /// </summary>
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// User's full legal name.
        /// </summary>
        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Role determines system privileges (Admin, Investigator, Officer, or Citizen).
        /// </summary>
        [Required, MaxLength(30)]
        public string Role { get; set; } = "Citizen";

        public ClearanceLevel ClearanceLevel { get; set; } = ClearanceLevel.Low;

        /// <summary>
        /// Timestamp when the user account was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Optional phone number for contact purposes.
        /// </summary>
        public string? PhoneNumber { get; set; }

        // 🔹 Navigation Properties
        public ICollection<Case>? CreatedCases { get; set; }
        public ICollection<Evidence>? AddedEvidence { get; set; }
        public ICollection<CaseComment> CaseComments { get; set; } = new List<CaseComment>();
    }
}
