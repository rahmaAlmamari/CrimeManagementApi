using CrimeManagementApi.Models;

namespace CrimeManagementApi.Repositories.Interfaces
{
    /// <summary>Evidence repo interface.</summary>
    public interface IEvidenceRepository : IGenericRepository<Evidence>
    {
        Task<IEnumerable<Evidence>> GetByCaseIdAsync(int caseId);
        Task<Evidence?> GetWithAuditAsync(int id);
    }
}
