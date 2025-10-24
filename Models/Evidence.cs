using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrimeManagementApi.Models
{
    [Table("evidence")]
    public class Evidence
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int CaseId { get; set; }

        [MaxLength(100)]
        public string Type { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? FilePath { get; set; }

        [MaxLength(50)]
        public string? MimeType { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }

        public int? AddedByUserId { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        //  Soft delete flags (important)
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        // 🔹 Navigation Properties
        [ForeignKey(nameof(AddedByUserId))]
        public User? AddedByUser { get; set; }

        [ForeignKey(nameof(CaseId))]
        public Case? Case { get; set; }

        public ICollection<EvidenceAuditLog>? AuditLogs { get; set; }

    }
}
