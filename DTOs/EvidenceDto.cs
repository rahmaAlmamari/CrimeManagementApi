using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CrimeManagementApi.DTOs
{
    // ============================================================
    // 🔹 EvidenceDto.cs — DTOs for evidence handling
    // ============================================================

    /// <summary>
    /// Represents evidence data returned to clients.
    /// This DTO omits sensitive database or audit details.
    /// </summary>
    public class EvidenceDto
    {
        public int Id { get; set; }
        public int CaseId { get; set; }
        public int AddedByUserId { get; set; }

        public string Type { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? FilePath { get; set; }
        public string? MimeType { get; set; }
        public string? Remarks { get; set; }

        public bool IsSoftDeleted { get; set; }
        public DateTime AddedAt { get; set; }

        // 🔹 User who added it
        public string? AddedByUserName { get; set; }

        // 🔹 Audit trail entries
        public ICollection<EvidenceAuditDto>? AuditLogs { get; set; }
    }

    // ============================================================
    // 🔹 CreateEvidenceDto.cs — Used when adding new evidence
    // ============================================================

    /// <summary>
    /// Structure for creating new evidence associated with a case.
    /// </summary>
    public class CreateEvidenceDto
    {
        public int CaseId { get; set; }
        public int AddedByUserId { get; set; }

        public string Type { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? FilePath { get; set; }
        public string? MimeType { get; set; }
        public string? Remarks { get; set; }
    }

    /// <summary>
    /// Represents an uploaded evidence file and its metadata.
    /// </summary>
    public class UploadEvidenceDto
    {
        [Required]
        public int CaseId { get; set; }

        [Required]
        public int AddedByUserId { get; set; }

        [MaxLength(100)]
        public string? Type { get; set; } = "Document";

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }

        [Required]
        public IFormFile File { get; set; } = null!;
    }
}

