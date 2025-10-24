using System.ComponentModel.DataAnnotations;

namespace CrimeManagementApi.DTOs
{
    // ============================================================
    // 🔹 CaseCommentDto.cs — DTOs for case-specific commenting
    // ============================================================

    /// <summary>
    /// Represents a comment linked to a specific case, created by an authorized user
    /// such as an Investigator, Officer, or Admin. Returned in APIs that fetch
    /// case discussions or audit-related notes.
    /// </summary>
    public class CaseCommentDto
    {
        /// <summary>Unique identifier for this comment.</summary>
        public int Id { get; set; }

        /// <summary>ID of the related case.</summary>
        [Required]
        public int CaseId { get; set; }

        /// <summary>ID of the user who posted the comment.</summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>Full name of the user who created the comment.</summary>
        [MaxLength(100)]
        public string? UserFullName { get; set; }

        /// <summary>Role of the user who created the comment (Investigator, Officer, Admin).</summary>
        [MaxLength(50)]
        public string? UserRole { get; set; }

        /// <summary>The actual comment text content.</summary>
        [Required]
        [StringLength(250, MinimumLength = 5,
            ErrorMessage = "Comment must be between 5 and 250 characters.")]
        public string Content { get; set; } = string.Empty;

        /// <summary>Indicates if this comment is soft deleted (hidden from view).</summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>Indicates if this comment is pinned for importance.</summary>
        public bool IsPinned { get; set; } = false;

        /// <summary>Date and time when this comment was created (UTC).</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Date and time when this comment was last updated (optional).</summary>
        public DateTime? UpdatedAt { get; set; }
    }

    // ============================================================
    // 🔹 CreateCaseCommentDto.cs — Used for creating new comments
    // ============================================================

    /// <summary>
    /// Represents the data structure required to create a new case comment.
    /// </summary>
    public class CreateCaseCommentDto
    {
        /// <summary>ID of the related case.</summary>
        [Required]
        public int CaseId { get; set; }

        /// <summary>ID of the user creating the comment.</summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>Actual comment text content.</summary>
        [Required]
        [StringLength(250, MinimumLength = 5,
            ErrorMessage = "Comment must be between 5 and 250 characters.")]
        public string Content { get; set; } = string.Empty;

        /// <summary> flag to pin the comment for visibility.</summary>
        public bool IsPinned { get; set; } = false;
    }

    // ============================================================
    // 🔹 UpdateCaseCommentDto.cs — Used for editing comments
    // ============================================================

    /// <summary>
    /// Used for updating or moderating existing case comments.
    /// </summary>
    public class UpdateCaseCommentDto
    {
        /// <summary> updated comment content (must follow same validation rules).</summary>
        [StringLength(250, MinimumLength = 5,
            ErrorMessage = "Comment must be between 5 and 250 characters.")]
        public string? Content { get; set; }

        /// <summary> flag to toggle comment deletion state (soft delete).</summary>
        public bool? IsDeleted { get; set; }

        /// <summary> flag to pin/unpin comment.</summary>
        public bool? IsPinned { get; set; }

        /// <summary>UTC timestamp for update time.</summary>
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
