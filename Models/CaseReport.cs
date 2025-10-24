using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrimeManagementApi.Models
{
    /// <summary>
    /// Represents a link between a Case and a Crime Report (composite key model).
    /// </summary>
    [Table("case_reports")]
    public class CaseReport
    {
        [Key]
        public int Id { get; set; } // keep if EF needs surrogate key, optional if composite key used

        [Required]
        [ForeignKey(nameof(Case))]
        public int CaseId { get; set; }

        [Required]
        [ForeignKey(nameof(Report))]
        public int ReportId { get; set; }

        /// <summary>
        /// Optional investigator or admin notes when linking the case and report.
        /// </summary>
        [MaxLength(500)]
        public string? Notes { get; set; }

        /// <summary>
        /// UTC timestamp when the link was created.
        /// </summary>
        public DateTime LinkedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Case Case { get; set; } = null!;
        public CrimeReport Report { get; set; } = null!;
    }
}
