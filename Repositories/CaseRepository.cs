using CrimeManagementApi.Data;
using CrimeManagementApi.Models;
using CrimeManagementApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrimeManagementApi.Repositories.Implementations
{
    /// <summary>Case repo implementation.</summary>
    public class CaseRepository : GenericRepository<Case>, ICaseRepository
    {
        private readonly AppDbContext _context;
        public CaseRepository(AppDbContext context) : base(context) => _context = context;

        public async Task<Case?> GetCaseDetailsAsync(int id)
            => await _context.Cases
                .Include(c => c.CreatedByUser)
                .Include(c => c.Assignees).ThenInclude(a => a.User)
                .Include(c => c.Participants).ThenInclude(p => p.Participant)
                .Include(c => c.EvidenceList)
                .Include(c => c.CaseComments).ThenInclude(cm => cm.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<IEnumerable<Case>> SearchCasesAsync(string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await _context.Cases.AsNoTracking().ToListAsync();

            keyword = keyword.ToLower();
            return await _context.Cases
                .Where(c => c.Name.ToLower().Contains(keyword)
                         || (c.Description != null && c.Description.ToLower().Contains(keyword)))
                .AsNoTracking().ToListAsync();
        }
    }
}
