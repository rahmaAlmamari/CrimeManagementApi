using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrimeManagementApi.Models
{
    [Table("case_participants")]
    public class CaseParticipant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Case")]
        public int CaseId { get; set; }

        [ForeignKey("Participant")]
        public int ParticipantId { get; set; }

        public int? AddedByUserId { get; set; }
        public string? Notes { get; set; }
        public DateTime LinkedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Case? Case { get; set; }
        public Participant? Participant { get; set; }
    }
}
