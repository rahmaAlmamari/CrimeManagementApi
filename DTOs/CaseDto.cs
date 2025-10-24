using CrimeManagementApi.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace CrimeManagementApi.DTOs
{
    // ============================================================
    // 🔹 CaseDto — Public-facing structure for API responses
    // ============================================================
    public class CaseDto
    {
        public int Id { get; set; }

        [Required, MaxLength(30)]
        public string CaseNumber { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required, MaxLength(50)]
        public string CaseType { get; set; } = "General";

        [Required]
        public ClearanceLevel ClearanceLevel { get; set; } = ClearanceLevel.Low;

        [Required, MaxLength(20)]
        public string Status { get; set; } = "Pending";

        [MaxLength(100)]
        public string? AreaCity { get; set; }

        public int CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public string? CreatedByUserRole { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// List of all evidence items related to this case.
        /// </summary>
        public List<EvidenceDto>? Evidences { get; set; }
    }

    // ============================================================
    // 🔹 CreateCaseDto — Used when creating new cases
    // ============================================================
    public class CreateCaseDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required, MaxLength(50)]
        public string CaseType { get; set; } = "General";

        [Required]
        public ClearanceLevel ClearanceLevel { get; set; } = ClearanceLevel.Low;

        [Required, MaxLength(20)]
        public string Status { get; set; } = "Pending";

        [MaxLength(100)]
        public string? AreaCity { get; set; }

        public int? CreatedByUserId { get; set; }
    }

    // ============================================================
    // 🔹 UpdateCaseDto — For editing existing cases
    // ============================================================
    public class UpdateCaseDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string? CaseType { get; set; }

        public ClearanceLevel? ClearanceLevel { get; set; }

        [MaxLength(20)]
        public string? Status { get; set; }

        [MaxLength(100)]
        public string? AreaCity { get; set; }
    }
}
