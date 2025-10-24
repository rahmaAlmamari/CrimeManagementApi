using AutoMapper;
using CrimeManagementApi.Data;
using CrimeManagementApi.DTOs;
using CrimeManagementApi.Models;
using CrimeManagementApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrimeManagementApi.Services.Implementations
{
    public class CaseParticipantService : ICaseParticipantService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CaseParticipantService> _logger;

        public CaseParticipantService(AppDbContext context, IMapper mapper, ILogger<CaseParticipantService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CaseParticipantDto?> LinkAsync(CreateCaseParticipantDto dto)
        {
            try
            {
                // prevent duplicate (same case + participant)
                bool exists = await _context.CaseParticipants
                    .AnyAsync(cp => cp.CaseId == dto.CaseId && cp.ParticipantId == dto.ParticipantId);

                if (exists)
                {
                    _logger.LogWarning("Participant {PId} already linked to Case {CId}", dto.ParticipantId, dto.CaseId);
                    return null;
                }

                // validate foreign keys
                bool caseExists = await _context.Cases.AnyAsync(c => c.Id == dto.CaseId);
                bool participantExists = await _context.Participants.AnyAsync(p => p.Id == dto.ParticipantId);

                if (!caseExists || !participantExists)
                {
                    _logger.LogWarning("Invalid CaseId ({CId}) or ParticipantId ({PId})", dto.CaseId, dto.ParticipantId);
                    return null;
                }

                var entity = _mapper.Map<CaseParticipant>(dto);
                entity.LinkedAt = DateTime.UtcNow;

                _context.CaseParticipants.Add(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("✅ Linked participant {PId} to case {CId}", dto.ParticipantId, dto.CaseId);

                return _mapper.Map<CaseParticipantDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error linking participant to case");
                return null;
            }
        }

        public async Task<IEnumerable<CaseParticipantDto>> GetAllAsync()
        {
            var list = await _context.CaseParticipants.ToListAsync();
            return _mapper.Map<IEnumerable<CaseParticipantDto>>(list);
        }

        public async Task<CaseParticipantDto?> GetByIdAsync(int id)
        {
            var entity = await _context.CaseParticipants.FindAsync(id);
            return entity == null ? null : _mapper.Map<CaseParticipantDto>(entity);
        }
    }
}
