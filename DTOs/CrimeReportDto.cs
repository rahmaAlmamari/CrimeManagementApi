using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CrimeManagementApi.DTOs
{
    // ============================================================
    // 🔹 CrimeReportDto.cs — DTOs for public crime reporting
    // ============================================================

    /// <summary>
    /// Represents the structure of crime reports shown to users and authorities.
    /// </summary>
    public class CrimeReportDto
    {
        /// <summary>Unique identifier for the crime report.</summary>
        public int Id { get; set; }

        /// <summary>Short title summarizing the incident.</summary>
        [Required, MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        /// <summary>Detailed description provided by the reporter (optional).</summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>City or area where the incident occurred (optional).</summary>
        [MaxLength(100)]
        public string? AreaCity { get; set; }

        /// <summary>Current case status (Pending, EnRoute, OnScene, UnderInvestigation, Resolved).</summary>
        [Required, MaxLength(50)]
        public string Status { get; set; } = "Pending";

        /// <summary>Date and time when the crime was reported (UTC).</summary>
        public DateTime ReportDateTime { get; set; } = DateTime.UtcNow;

        /// <summary> latitude for geolocation tracking.</summary>
        [Range(-90, 90)]
        public decimal? Latitude { get; set; }

        /// <summary> longitude for geolocation tracking.</summary>
        [Range(-180, 180)]
        public decimal? Longitude { get; set; }
    }

    // ============================================================
    // 🔹 CreateCrimeReportDto.cs — Used when submitting a new report
    // ============================================================

    /// <summary>
    /// Represents the structure used by citizens or users to report new crimes.
    /// </summary>
    public class CreateCrimeReportDto
    {
        /// <summary>Title describing the reported crime.</summary>
        [Required, MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        /// <summary> detailed description of the crime.</summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary> area or city where the crime took place.</summary>
        [MaxLength(100)]
        [AllowNull]
        public string? AreaCity { get; set; }

        /// <summary>ID of the user submitting the report (null if anonymous).</summary>
        [AllowNull]
        public int? ReportedByUserId { get; set; }

        /// <summary> latitude for location tracking.</summary>
        [Range(-90, 90)]
        public decimal? Latitude { get; set; }

        /// <summary> longitude for location tracking.</summary>
        [Range(-180, 180)]
        public decimal? Longitude { get; set; }
    }

    // ============================================================
    // 🔹 UpdateCrimeReportDto.cs — Used for administrative updates
    // ============================================================

    /// <summary>
    /// Used by investigators or admins to update existing crime reports.
    /// </summary>
    public class UpdateCrimeReportDto
    {
        /// <summary> updated title.</summary>
        [MaxLength(100)]
        public string? Title { get; set; }

        /// <summary> updated description.</summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary> updated area or city.</summary>
        [MaxLength(100)]
        public string? AreaCity { get; set; }

        /// <summary> updated status (e.g., UnderInvestigation, Resolved).</summary>
        [MaxLength(50)]
        public string? Status { get; set; }

        /// <summary> updated latitude (if relocated or corrected).</summary>
        [Range(-90, 90)]
        public decimal? Latitude { get; set; }

        /// <summary> updated longitude (if relocated or corrected).</summary>
        [Range(-180, 180)]
        public decimal? Longitude { get; set; }
    }
}
