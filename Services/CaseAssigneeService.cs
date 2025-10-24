using AutoMapper;
using CrimeManagementApi.Data;
using CrimeManagementApi.DTOs;
using CrimeManagementApi.Models;
using CrimeManagementApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrimeManagementApi.Services
{
    /// <summary>
    /// Manages case assignment logic for officers/investigators.
    /// </summary>
    public class CaseAssigneeService : ICaseAssigneeService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CaseAssigneeService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CaseAssigneeDto>> GetByCaseIdAsync(int caseId)
        {
            var list = await _context.CaseAssignees
                .Include(a => a.User)
                .Where(a => a.CaseId == caseId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CaseAssigneeDto>>(list);
        }

        public async Task<CaseAssigneeDto?> AssignAsync(CreateCaseAssigneeDto dto)
        {
            var caseEntity = await _context.Cases.FindAsync(dto.CaseId);
            var user = await _context.Users.FindAsync(dto.UserId);

            if (caseEntity == null || user == null)
                return null;

            // 🔹 Officer Clearance Validation
            if (user.ClearanceLevel < caseEntity.ClearanceLevel)
                throw new Exception($"User clearance ({user.ClearanceLevel}) too low for this case ({caseEntity.ClearanceLevel}).");

            var entity = _mapper.Map<CaseAssignee>(dto);
            entity.AssignedAt = DateTime.UtcNow;

            _context.CaseAssignees.Add(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<CaseAssigneeDto>(entity);
        }

        public async Task<CaseAssigneeDto?> UpdateProgressAsync(int id, string progressStatus)
        {
            var entity = await _context.CaseAssignees.FindAsync(id);
            if (entity == null) return null;

            entity.ProgressStatus = progressStatus;
            await _context.SaveChangesAsync();

            return _mapper.Map<CaseAssigneeDto>(entity);
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var entity = await _context.CaseAssignees.FindAsync(id);
            if (entity == null) return false;

            _context.CaseAssignees.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
