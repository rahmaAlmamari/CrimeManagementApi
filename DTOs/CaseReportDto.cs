using System.ComponentModel.DataAnnotations;

namespace CrimeManagementApi.DTOs
{
    // ============================================================
    // 🔹 CaseReportDto.cs — DTOs for linking cases with reports
    // ============================================================

    /// <summary>
    /// Represents the link between a citizen-submitted crime report
    /// and an official case handled by investigators.
    /// </summary>
    public class CaseReportDto
    {
        /// <summary>Unique identifier for this link.</summary>
        public int Id { get; set; }

        /// <summary>ID of the linked case.</summary>
        [Required]
        public int CaseId { get; set; }

        /// <summary>Case number or name (for quick display).</summary>
        [MaxLength(100)]
        public string? CaseName { get; set; }

        /// <summary>ID of the linked crime report.</summary>
        [Required]
        public int ReportId { get; set; }

        /// <summary>Title of the linked crime report (citizen side).</summary>
        [MaxLength(150)]
        public string? ReportTitle { get; set; }

        /// <summary>
        /// City or area of the incident.
        /// </summary>
        [MaxLength(100)]
        public string? AreaCity { get; set; }

        /// <summary>Date and time this case-report link was created (UTC).</summary>
        public DateTime LinkedAt { get; set; } = DateTime.UtcNow;

        /// <summary> notes or linkage remarks added by the investigator/admin.</summary>
        [MaxLength(500)]
        public string? Notes { get; set; }
    }

    // ============================================================
    // 🔹 CreateCaseReportDto.cs — Used for creating new links
    // ============================================================

    /// <summary>
    /// DTO used to link an existing crime report to a new or existing case.
    /// </summary>
    public class CreateCaseReportDto
    {
        /// <summary>ID of the case to associate with the report.</summary>
        [Required]
        public int CaseId { get; set; }

        /// <summary>ID of the report being linked.</summary>
        [Required]
        public int ReportId { get; set; }

        /// <summary> linkage notes for administrative tracking.</summary>
        [MaxLength(500)]
        public string? Notes { get; set; }
    }

    // ============================================================
    // 🔹 UpdateCaseReportDto.cs — Used for updating links
    // ============================================================

    /// <summary>
    /// DTO used to modify linkage data (for admin adjustments).
    /// </summary>
    public class UpdateCaseReportDto
    {
        /// <summary> update for re-linking to another case.</summary>
        public int? CaseId { get; set; }

        /// <summary> update for re-linking to another report.</summary>
        public int? ReportId { get; set; }

        /// <summary> updated notes for the link record.</summary>
        [MaxLength(500)]
        public string? Notes { get; set; }

        /// <summary>Timestamp of last update (UTC).</summary>
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
