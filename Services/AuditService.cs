using AutoMapper;
using CrimeManagementApi.Data;
using CrimeManagementApi.DTOs;
using CrimeManagementApi.Models;
using CrimeManagementApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrimeManagementApi.Services
{
    /// <summary>
    /// Handles all evidence audit logging and retrieval operations.
    /// </summary>
    public class AuditService : IAuditService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AuditService> _logger;

        public AuditService(AppDbContext context, IMapper mapper, ILogger<AuditService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // ==========================================================
        // 🔹 Add a new audit log entry
        // ==========================================================
        public async Task AddLogAsync(EvidenceAuditDto dto)
        {
            try
            {
                if (dto == null)
                    throw new ArgumentNullException(nameof(dto));

                var entity = _mapper.Map<EvidenceAuditLog>(dto);
                entity.ActedAt = dto.ActedAt == default ? DateTime.UtcNow : dto.ActedAt;

                _context.EvidenceAuditLogs.Add(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "✅ Audit Log Added: EvidenceId={Eid}, Action={Action}, User={Uid}",
                    entity.EvidenceId, entity.Action, entity.ActedByUserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to add audit log for Evidence ID {Eid}", dto?.EvidenceId);
                throw;
            }
        }

        // ==========================================================
        // 🔹 Get all audit logs
        // ==========================================================
        public async Task<IEnumerable<EvidenceAuditDto>> GetAllAsync()
        {
            var logs = await _context.EvidenceAuditLogs
                .Include(l => l.ActedByUser)
                .Include(l => l.Evidence)
                .OrderByDescending(l => l.ActedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<EvidenceAuditDto>>(logs);
        }

        // ==========================================================
        // 🔹 Get audit logs by Evidence ID
        // ==========================================================
        public async Task<IEnumerable<EvidenceAuditDto>> GetByEvidenceIdAsync(int evidenceId)
        {
            var logs = await _context.EvidenceAuditLogs
                .Where(l => l.EvidenceId == evidenceId)
                .Include(l => l.ActedByUser)
                .Include(l => l.Evidence)
                .OrderByDescending(l => l.ActedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<EvidenceAuditDto>>(logs);
        }

        // ==========================================================
        // 🔹 Get audit logs by User ID
        // ==========================================================
        public async Task<IEnumerable<EvidenceAuditDto>> GetByUserIdAsync(int userId)
        {
            var logs = await _context.EvidenceAuditLogs
                .Where(l => l.ActedByUserId == userId)
                .Include(l => l.Evidence)
                .OrderByDescending(l => l.ActedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<EvidenceAuditDto>>(logs);
        }
    }
}
