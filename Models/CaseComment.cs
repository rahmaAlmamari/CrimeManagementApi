using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrimeManagementApi.Models
{
    /// <summary>
    /// Represents a comment posted on a specific case by an officer, investigator, or admin.
    /// </summary>
    [Table("case_comments")]
    public class CaseComment
    {
        /// <summary>
        /// Unique identifier for this case comment.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// ID of the related case.
        /// </summary>
        [Required]
        [ForeignKey(nameof(Case))]
        public int CaseId { get; set; }

        /// <summary>
        /// The case entity associated with this comment.
        /// </summary>
        public Case Case { get; set; } = null!;

        /// <summary>
        /// ID of the user who added the comment.
        /// </summary>
        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        /// <summary>
        /// The user entity who authored this comment.
        /// </summary>
        public User User { get; set; } = null!;

        /// <summary>
        /// Actual comment content.
        /// </summary>
        [Required(ErrorMessage = "Comment content is required.")]
        [StringLength(250, MinimumLength = 5,
            ErrorMessage = "Comment must be between 5 and 250 characters.")]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// When the comment was created (UTC).
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// When the comment was last updated (optional).
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Flag for soft deletion (comment hidden but not removed).
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Role of the user who posted the comment (Investigator, Officer, Admin).
        /// Stored for historical tracking.
        /// </summary>
        [StringLength(50)]
        public string? UserRole { get; set; }

        /// <summary>
        /// Optional flag to mark comment as pinned/important.
        /// </summary>
        public bool IsPinned { get; set; } = false;
    }
}
