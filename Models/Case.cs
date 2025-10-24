using CrimeManagementApi.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrimeManagementApi.Models
{
    /// <summary>
    /// Represents a formal criminal case record in the Crime Management System.
    /// </summary>
    [Table("cases")]
    public class Case
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Unique case reference number (system or investigator-assigned).
        /// </summary>
        [Required, MaxLength(30)]
        public string CaseNumber { get; set; } = string.Empty;

        /// <summary>
        /// Short, descriptive case title.
        /// </summary>
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Full case description including summary of events or allegations.
        /// </summary>
        [Required, MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Geographic area or city where the case occurred.
        /// </summary>
        [MaxLength(50)]
        public string? AreaCity { get; set; }

        /// <summary>
        /// Case category (e.g., General, Robbery, Homicide, Kidnapping).
        /// </summary>
        [MaxLength(50)]
        public string CaseType { get; set; } = "General";

        /// <summary>
        /// Sensitivity level that determines which users can access the case.
        /// </summary>
        public ClearanceLevel ClearanceLevel { get; set; } = ClearanceLevel.Low;

        /// <summary>
        /// Current case status.
        /// </summary>
        [Required, MaxLength(30)]
        public string Status { get; set; } = "Pending"; // Pending, Ongoing, Closed

        /// <summary>
        /// Foreign key of the user who created this case.
        /// </summary>
        [Required]
        public int CreatedByUserId { get; set; }

        [ForeignKey(nameof(CreatedByUserId))]
        public User? CreatedByUser { get; set; }

        /// <summary>
        /// Case creation timestamp (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // 🔹 Navigation Properties
        public ICollection<Evidence>? EvidenceList { get; set; } = new List<Evidence>();
        public ICollection<CaseAssignee>? Assignees { get; set; } = new List<CaseAssignee>();
        public ICollection<CaseParticipant>? Participants { get; set; } = new List<CaseParticipant>();
        public ICollection<CaseReport> CaseReports { get; set; } = new List<CaseReport>();
        public ICollection<CaseComment> CaseComments { get; set; } = new List<CaseComment>();
    }
}
