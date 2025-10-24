using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrimeManagementApi.Models
{
    [Table("evidence_audit_logs")]
    public class EvidenceAuditLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // 🔹 Foreign keys
        public int EvidenceId { get; set; }
        public int? ActedByUserId { get; set; } 

        // 🔹 Audit info
        [Required, MaxLength(50)]
        public string Action { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Details { get; set; }

        public DateTime ActedAt { get; set; } = DateTime.UtcNow;

        // 🔹 Navigation
        public Evidence? Evidence { get; set; }

        [ForeignKey(nameof(ActedByUserId))]
        public User? ActedByUser { get; set; }
    }
}
