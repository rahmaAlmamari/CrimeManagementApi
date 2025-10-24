using CrimeManagementApi.Models;

namespace CrimeManagementApi.Repositories.Interfaces
{
    /// <summary>Case repo interface.</summary>
    public interface ICaseRepository : IGenericRepository<Case>
    {
        Task<Case?> GetCaseDetailsAsync(int id);
        Task<IEnumerable<Case>> SearchCasesAsync(string? keyword);
    }
}
