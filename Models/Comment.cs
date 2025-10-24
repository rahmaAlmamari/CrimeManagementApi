using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrimeManagementApi.Models
{
    /// <summary>
    /// Represents a general system comment (for discussions, audit notes, or feedback).
    /// </summary>
    [Table("comments")]
    public class Comment
    {
        /// <summary>
        /// Unique identifier for the comment.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// The actual text content of the comment.
        /// </summary>
        [Required]
        [StringLength(300, MinimumLength = 3)]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the user who created this comment.
        /// </summary>
        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        /// <summary>
        /// The user entity who made the comment.
        /// </summary>
        public User User { get; set; } = null!;

        /// <summary>
        /// Date and time when the comment was created (UTC).
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Indicates whether this comment is pinned for prominence.
        /// </summary>
        public bool IsPinned { get; set; } = false;

        /// <summary>
        /// Indicates whether this comment has been soft-deleted.
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Optional reference for associating the comment with an external entity (like case, evidence, etc.).
        /// </summary>
        [StringLength(50)]
        public string? ReferenceType { get; set; }

        /// <summary>
        /// Optional ID reference for external entity (e.g., CaseId, EvidenceId).
        /// </summary>
        public int? ReferenceId { get; set; }

        /// <summary>
        /// Last updated timestamp, if modified.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
