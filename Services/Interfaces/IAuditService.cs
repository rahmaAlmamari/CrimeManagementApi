using CrimeManagementApi.DTOs;

namespace CrimeManagementApi.Services.Interfaces
{
    /// <summary>
    /// Defines operations for managing and retrieving evidence audit logs.
    /// </summary>
    public interface IAuditService
    {
        /// <summary>Add a new audit log entry.</summary>
        Task AddLogAsync(EvidenceAuditDto dto);

        /// <summary>Retrieve all audit logs in the system.</summary>
        Task<IEnumerable<EvidenceAuditDto>> GetAllAsync();

        /// <summary>Retrieve all audit logs for a specific evidence ID.</summary>
        Task<IEnumerable<EvidenceAuditDto>> GetByEvidenceIdAsync(int evidenceId);

        /// <summary>Retrieve all audit logs performed by a specific user.</summary>
        Task<IEnumerable<EvidenceAuditDto>> GetByUserIdAsync(int userId);
    }
}
