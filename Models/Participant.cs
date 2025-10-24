using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrimeManagementApi.Models
{
    [Table("participants")]
    public class Participant
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Role { get; set; } 

        [MaxLength(15)]
        public string? Phone { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public int? CaseId { get; set; }
        public Case? Case { get; set; }

        // user who added participant (officer/investigator)
        public int? AddedByUserId { get; set; }
        public User? AddedByUser { get; set; }

        public DateTime AddedOn { get; set; } = DateTime.UtcNow;

        // navigation to many-to-many bridge
        public ICollection<CaseParticipant> CaseParticipants { get; set; } = new List<CaseParticipant>();
    }
}
