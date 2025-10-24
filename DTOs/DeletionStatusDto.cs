using System.ComponentModel.DataAnnotations;

namespace CrimeManagementApi.DTOs
{
    // ============================================================
    // 🔹 DeletionStatusDto.cs — DTOs for evidence hard-delete tracking
    // ============================================================

    /// <summary>
    /// Represents the current runtime state of a hard deletion process
    /// for a specific evidence item. Used for long-polling progress tracking.
    /// </summary>
    public class DeletionStatusDto
    {
        /// <summary>The ID of the evidence being deleted.</summary>
        [Required]
        public int EvidenceId { get; set; }

        /// <summary>
        /// The current status of the deletion process.
        /// Allowed values: Pending, InProgress, Completed, Failed.
        /// </summary>
        [Required, MaxLength(50)]
        public string Status { get; set; } = "Pending";

        /// <summary>UTC timestamp when the status was last updated.</summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Optional message describing the current deletion phase.</summary>
        [MaxLength(200)]
        public string? Message { get; set; }
    }

    // ============================================================
    // 🔹 StartDeletionRequestDto.cs — Used to initiate hard deletion
    // ============================================================

    /// <summary>
    /// Represents a request from an admin to initiate a hard delete
    /// operation for a specific evidence entry.
    /// </summary>
    public class StartDeletionRequestDto
    {
        /// <summary>ID of the evidence item to permanently delete.</summary>
        [Required]
        public int EvidenceId { get; set; }

        /// <summary>ID of the admin initiating the deletion.</summary>
        [Required]
        public int RequestedByUserId { get; set; }

        /// <summary>
        /// Confirmation response — must be "yes" to proceed with the deletion.
        /// </summary>
        [Required]
        [RegularExpression("^(yes|no)$", ErrorMessage = "Confirmation must be 'yes' or 'no'.")]
        public string Confirmation { get; set; } = "no";
    }

    // ============================================================
    // 🔹 DeletionResultDto.cs — Used to return final deletion outcome
    // ============================================================

    /// <summary>
    /// Represents the final outcome of a deletion process,
    /// returned after the operation completes or fails.
    /// </summary>
    public class DeletionResultDto
    {
        /// <summary>ID of the evidence that was deleted.</summary>
        public int EvidenceId { get; set; }

        /// <summary>Whether the deletion succeeded.</summary>
        public bool IsSuccessful { get; set; }

        /// <summary>Any message describing the result (success/failure details).</summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>UTC timestamp marking when the process finished.</summary>
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    }
}
