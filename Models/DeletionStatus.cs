using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrimeManagementApi.Models
{
    /// <summary>
    /// Represents the runtime or persistent state of a hard deletion process.
    /// Tracks progress and update time for evidence deletion tasks.
    /// </summary>
    [Table("deletion_status")]
    public class DeletionStatus
    {
        /// <summary>
        /// Unique identifier for the record (optional if transient).
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// The ID of the evidence being deleted.
        /// </summary>
        [Required]
        public int EvidenceId { get; set; }

        /// <summary>
        /// Current status of the deletion operation.
        /// </summary>
        [Required, MaxLength(20)]
        public string Status { get; set; } = "Pending";
        // Possible values: Pending | InProgress | Completed | Failed

        /// <summary>
        /// Optional message containing extra info (e.g., errors or confirmation notes).
        /// </summary>
        [MaxLength(500)]
        public string? Message { get; set; }

        /// <summary>
        /// UTC timestamp of the last update to this deletion state.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
