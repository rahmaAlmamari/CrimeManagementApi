using System;
using System.ComponentModel.DataAnnotations;

namespace CrimeManagementApi.DTOs
{
    // ============================================================
    // 🔹 CaseParticipantDto — returned in API responses
    // ============================================================
    public class CaseParticipantDto
    {
        public int Id { get; set; }
        public int CaseId { get; set; }
        public int ParticipantId { get; set; }
        public DateTime LinkedAt { get; set; }
        public string? Notes { get; set; }
    }

    // ============================================================
    // 🔹 CreateCaseParticipantDto — for linking participants to cases
    // ============================================================


    public class CreateCaseParticipantDto
        {
            public int CaseId { get; set; }
            public int ParticipantId { get; set; }
            public int? AddedByUserId { get; set; }
            public string? Notes { get; set; }
        }
    


    // ============================================================
    // 🔹 UpdateCaseParticipantDto — for editing case link details
    // ============================================================
    public class UpdateCaseParticipantDto
    {
        public int? CaseId { get; set; }

        public int? ParticipantId { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
