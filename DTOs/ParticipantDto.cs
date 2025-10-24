using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CrimeManagementApi.DTOs
{
    // ============================================================
    // 🔹 ParticipantDto — returned in API responses
    // ============================================================
    public class ParticipantDto
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Role { get; set; }    //  "Victim", "Suspect", etc.

        [MaxLength(15)]
        public string? Phone { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public int CaseId { get; set; }

        public int? AddedByUserId { get; set; }

        public DateTime AddedOn { get; set; }
    }

    // ============================================================
    // 🔹 CreateParticipantDto — for creating new participants
    // ============================================================
    public class CreateParticipantDto
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Role { get; set; }

        [MaxLength(15)]
        public string? Phone { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required]
        public int CaseId { get; set; }

        [AllowNull]
        public int? AddedByUserId { get; set; }
    }

    // ============================================================
    // 🔹 UpdateParticipantDto — for PATCH/PUT updates
    // ============================================================
    public class UpdateParticipantDto
    {
        [MaxLength(100)]
        public string? FullName { get; set; }

        [MaxLength(50)]
        public string? Role { get; set; }

        [MaxLength(15)]
        public string? Phone { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public int? CaseId { get; set; }

        public int? AddedByUserId { get; set; }

        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
