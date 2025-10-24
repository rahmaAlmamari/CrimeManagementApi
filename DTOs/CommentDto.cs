using System.ComponentModel.DataAnnotations;

namespace CrimeManagementApi.DTOs
{
    // ============================================================
    // 🔹 CommentDto.cs — DTOs for system-wide comment management
    // ============================================================

    /// <summary>
    /// Represents a generic comment that can be linked to any case or evidence item.
    /// Used for discussions, audit notes, or system-wide communication.
    /// </summary>
    public class CommentDto
    {
        /// <summary>Unique identifier of the comment.</summary>
        public int Id { get; set; }

        /// <summary>
        /// ID of the related case or entity (e.g., evidence, report).
        /// </summary>
        [Required]
        public int CaseId { get; set; }

        /// <summary>ID of the user who posted this comment.</summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// The text content of the comment.
        /// Must be between 5 and 150 characters.
        /// </summary>
        [Required]
        [StringLength(150, MinimumLength = 5,
            ErrorMessage = "Comment must be between 5 and 150 characters.")]
        public string Content { get; set; } = string.Empty;

        /// <summary>Display name of the comment author (for UI use).</summary>
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        /// <summary>Role of the author (Admin, Investigator, Officer).</summary>
        [MaxLength(50)]
        public string? UserRole { get; set; }

        /// <summary>Indicates if the comment is pinned by an admin or investigator.</summary>
        public bool IsPinned { get; set; } = false;

        /// <summary>Indicates whether the comment has been soft deleted.</summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>UTC timestamp when the comment was created.</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>UTC timestamp when the comment was last updated (optional).</summary>
        public DateTime? UpdatedAt { get; set; }
    }

    // ============================================================
    // 🔹 CreateCommentDto.cs — Used for adding new comments
    // ============================================================

    /// <summary>
    /// Used to add a new comment to a case, evidence, or discussion thread.
    /// </summary>
    public class CreateCommentDto
    {
        public int CaseId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = string.Empty;
    }

    // ============================================================
    // 🔹 UpdateCommentDto.cs — Used for editing existing comments
    // ============================================================

    /// <summary>
    /// Represents the structure for updating or moderating a comment.
    /// </summary>
    public class UpdateCommentDto
    {
        /// <summary>Optional new content for the comment (if edited).</summary>
        [StringLength(150, MinimumLength = 5,
            ErrorMessage = "Comment must be between 5 and 150 characters.")]
        public string? Content { get; set; }

        /// <summary>Optional flag to pin/unpin a comment.</summary>
        public bool? IsPinned { get; set; }

        /// <summary>Optional flag to mark comment as deleted or restored.</summary>
        public bool? IsDeleted { get; set; }

        /// <summary>UTC timestamp of when the update occurred.</summary>
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
