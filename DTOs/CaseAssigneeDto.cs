using System.ComponentModel.DataAnnotations;

namespace CrimeManagementApi.DTOs
{
    // ============================================================
    // 🔹 CaseAssigneeDto.cs — DTOs for managing case assignments
    // ============================================================

    /// <summary>
    /// Represents an officer or investigator assigned to a specific case.
    /// Returned in APIs for tracking case responsibility and progress.
    /// </summary>
    public class CaseAssigneeDto
    {
        /// <summary>Unique identifier for the case assignment.</summary>
        public int Id { get; set; }

        /// <summary>ID of the related case.</summary>
        [Required]
        public int CaseId { get; set; }

        /// <summary>ID of the assigned user (officer or investigator).</summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>Display name of the assigned user.</summary>
        [MaxLength(100)]
        public string? UserFullName { get; set; }

        /// <summary>
        /// The role assigned within the case (e.g., Officer, Investigator).
        /// </summary>
        [Required, MaxLength(50)]
        public string AssignedRole { get; set; } = "Officer";


        public string ClearanceLevel { get; set; } = string.Empty;

        /// <summary>
        /// Current progress state of the assignee’s task.
        /// Allowed values: Pending, Ongoing, Closed.
        /// </summary>
        [Required, MaxLength(50)]
        public string ProgressStatus { get; set; } = "Pending";

        /// <summary>Date and time when this assignment was created (UTC).</summary>
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Date and time when the record was last updated (optional).</summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary> field for remarks or investigation notes.</summary>
        [MaxLength(500)]
        public string? Remarks { get; set; }
    }

    // ============================================================
    // 🔹 CreateCaseAssigneeDto.cs — Used for adding new assignments
    // ============================================================

    /// <summary>
    /// Represents data required when assigning a new investigator or officer to a case.
    /// </summary>
    public class CreateCaseAssigneeDto
    {
        /// <summary>ID of the case to assign the user to.</summary>
        [Required]
        public int CaseId { get; set; }

        /// <summary>ID of the user being assigned.</summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>Role assigned to the user in the case (e.g., Investigator, Officer).</summary>
        [Required, MaxLength(50)]
        public string AssignedRole { get; set; } = "Officer";

        /// <summary>Initial progress status of the assignment.</summary>
        [Required, MaxLength(50)]
        public string ProgressStatus { get; set; } = "Pending";

        /// <summary> remarks or assignment details.</summary>
        [MaxLength(500)]
        public string? Remarks { get; set; }
    }

    // ============================================================
    // 🔹 UpdateCaseAssigneeDto.cs — Used for updating assignments
    // ============================================================

    /// <summary>
    /// Used to update an existing case assignment (progress or reassignment).
    /// </summary>
    public class UpdateCaseAssigneeDto
    {
        /// <summary> update to the assigned role (Officer, Investigator).</summary>
        [MaxLength(50)]
        public string? AssignedRole { get; set; }

        /// <summary> update to the progress status (Pending, Ongoing, Closed).</summary>
        [MaxLength(50)]
        public string? ProgressStatus { get; set; }

        /// <summary> remarks about the update (e.g., "Transferred to another investigator").</summary>
        [MaxLength(500)]
        public string? Remarks { get; set; }

        /// <summary>UTC timestamp for when the update occurred.</summary>
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
