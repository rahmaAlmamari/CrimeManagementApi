using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrimeManagementApi.Models
{
    /// <summary>
    /// Represents a crime incident reported by a citizen or user before being escalated to a formal case.
    /// </summary>
    [Table("crime_reports")]
    public class CrimeReport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Short title or summary of the reported crime.
        /// </summary>
        [Required, MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of the incident.
        /// </summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Geographic area or city where the incident occurred.
        /// </summary>
        [MaxLength(50)]
        public string? AreaCity { get; set; }

        /// <summary>
        /// Date and time the report was submitted (stored as UTC).
        /// </summary>
        public DateTime ReportDateTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Current report status.
        /// </summary>
        [Required, MaxLength(50)]
        public string Status { get; set; } = "Pending";
        // Pending, EnRoute, OnScene, UnderInvestigation, Resolved

        /// <summary>
        /// Optional user who submitted the report (if registered).
        /// </summary>
        public int? ReportedByUserId { get; set; }

        [ForeignKey(nameof(ReportedByUserId))]
        public User? ReportedByUser { get; set; }

        /// <summary>
        /// GPS latitude coordinate of the incident location.
        /// </summary>
        [Column(TypeName = "decimal(10,8)")]
        public decimal? Latitude { get; set; }

        /// <summary>
        /// GPS longitude coordinate of the incident location.
        /// </summary>
        [Column(TypeName = "decimal(11,8)")]
        public decimal? Longitude { get; set; }

        /// <summary>
        /// Links this report to one or more formal cases.
        /// </summary>
        public ICollection<CaseReport> CaseReports { get; set; } = new List<CaseReport>();
    }
}
