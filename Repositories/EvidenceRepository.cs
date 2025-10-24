using CrimeManagementApi.Data;
using CrimeManagementApi.Models;
using CrimeManagementApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrimeManagementApi.Repositories.Implementations
{
    /// <summary>Evidence repo implementation.</summary>
    public class EvidenceRepository : GenericRepository<Evidence>, IEvidenceRepository
    {
        private readonly AppDbContext _context;
        public EvidenceRepository(AppDbContext context) : base(context) => _context = context;

        public async Task<IEnumerable<Evidence>> GetByCaseIdAsync(int caseId)
            => await _context.Evidence
                .Where(e => e.CaseId == caseId && !e.IsDeleted)
                .Include(e => e.AddedByUser)
                .OrderByDescending(e => e.AddedAt)
                .AsNoTracking().ToListAsync();

        public async Task<Evidence?> GetWithAuditAsync(int id)
            => await _context.Evidence
                .Include(e => e.AddedByUser)
                .Include(e => e.AuditLogs).ThenInclude(l => l.ActedByUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
    }
}
