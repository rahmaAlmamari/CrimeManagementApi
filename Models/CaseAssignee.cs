using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrimeManagementApi.Models
{
    /// <summary>
    /// Represents a mapping between a case and the assigned investigator or officer.
    /// Each assignment includes the user's role, progress status, and assigned date.
    /// </summary>
    [Table("case_assignees")]
    public class CaseAssignee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// The ID of the associated case.
        /// </summary>
        [Required]
        public int CaseId { get; set; }

        [ForeignKey(nameof(CaseId))]
        public Case? Case { get; set; }

        /// <summary>
        /// The ID of the user (investigator or officer) assigned to this case.
        /// </summary>
        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        /// <summary>
        /// Role of the assigned user in the case (Investigator, Officer).
        /// </summary>
        [Required, MaxLength(30)]
        public string AssignedRole { get; set; } = "Officer";

        /// <summary>
        /// Progress status of the assignee’s work (Pending, Ongoing, Closed).
        /// </summary>
        [Required, MaxLength(30)]
        public string ProgressStatus { get; set; } = "Pending";

        /// <summary>
        /// Timestamp when the user was assigned to the case (UTC).
        /// </summary>
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}
