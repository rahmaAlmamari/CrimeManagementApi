using AutoMapper;
using CrimeManagementApi.Data;
using CrimeManagementApi.DTOs;
using CrimeManagementApi.Models;
using CrimeManagementApi.Models.Enums;
using CrimeManagementApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrimeManagementApi.Services
{
    /// <summary>Manages CRUD and search for criminal cases.</summary>
    public class CaseService : ICaseService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;

        public CaseService(AppDbContext context, IMapper mapper, IEmailService emailService, ISmsService smsService)
        {
            _context = context;
            _mapper = mapper;
            _emailService = emailService;
            _smsService = smsService;
        }


        public async Task<IEnumerable<CaseDto>> GetAllAsync()
        {
            var cases = await _context.Cases.Include(c => c.CreatedByUser).ToListAsync();
            return _mapper.Map<IEnumerable<CaseDto>>(cases);
        }

        public async Task<CaseDto?> GetByIdAsync(int id)
        {
            var caseEntity = await _context.Cases
                .Include(c => c.CreatedByUser)
                .Include(c => c.EvidenceList)
                .Include(c => c.Assignees)
                .FirstOrDefaultAsync(c => c.Id == id);

            return caseEntity is null ? null : _mapper.Map<CaseDto>(caseEntity);
        }

        public async Task<CaseDto?> CreateAsync(CreateCaseDto dto)
        {
            var entity = _mapper.Map<Case>(dto);
            entity.CaseNumber = $"CASE-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

            _context.Cases.Add(entity);
            await _context.SaveChangesAsync();

            //  Notify citizens of new case in their city via Email
            var citizens = await _context.Users
                .Where(u => u.Role == "Citizen")
                .ToListAsync();

            foreach (var citizen in citizens)
            {
                await _emailService.SendNewCrimeAlertAsync(
                    to: citizen.Email,
                    area: entity.AreaCity ?? "Unknown Area",
                    description: $"New case reported: {entity.Name} - {entity.Description}"
                );
            }

            // 🔹 NEW: Send SMS notifications to assigned officers
            if (entity.Assignees != null && entity.Assignees.Any())
            {
                foreach (var assignee in entity.Assignees)
                {
                    var officer = await _context.Users.FindAsync(assignee.UserId);
                    if (officer != null && !string.IsNullOrEmpty(officer.PhoneNumber))
                    {
                        await _smsService.SendNewCaseAssignmentSmsAsync(
                            officer.PhoneNumber,
                            entity.CaseNumber,
                            officer.FullName
                        );
                    }
                }
            }

            return _mapper.Map<CaseDto>(entity);
        }


        public async Task<CaseDto?> UpdateAsync(int id, UpdateCaseDto dto)
        {
            var existingCase = await _context.Cases
                .Include(c => c.Assignees)
                .ThenInclude(a => a.User)
                .Include(c => c.CreatedByUser)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingCase == null) return null;

            string? oldStatus = existingCase.Status;

            // Apply updates only for fields that are not null
            if (!string.IsNullOrWhiteSpace(dto.Name))
                existingCase.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                existingCase.Description = dto.Description;

            if (!string.IsNullOrWhiteSpace(dto.CaseType))
                existingCase.CaseType = dto.CaseType;

            if (dto.ClearanceLevel.HasValue)
                existingCase.ClearanceLevel = dto.ClearanceLevel.Value;

            if (!string.IsNullOrWhiteSpace(dto.Status))
                existingCase.Status = dto.Status;

            if (!string.IsNullOrWhiteSpace(dto.AreaCity))
                existingCase.AreaCity = dto.AreaCity;

            await _context.SaveChangesAsync();

            // 🔹 NEW: Send SMS notification for status changes
            if (!string.IsNullOrWhiteSpace(dto.Status) && dto.Status != oldStatus)
            {
                // Notify assigned officers
                if (existingCase.Assignees != null)
                {
                    foreach (var assignee in existingCase.Assignees)
                    {
                        if (!string.IsNullOrEmpty(assignee.User?.PhoneNumber))
                        {
                            await _smsService.SendCaseUpdateSmsAsync(
                                assignee.User.PhoneNumber,
                                existingCase.CaseNumber,
                                existingCase.Status
                            );
                        }
                    }
                }

                // Notify case creator if they have a phone number
                if (existingCase.CreatedByUser != null && !string.IsNullOrEmpty(existingCase.CreatedByUser.PhoneNumber))
                {
                    await _smsService.SendCaseUpdateSmsAsync(
                        existingCase.CreatedByUser.PhoneNumber,
                        existingCase.CaseNumber,
                        existingCase.Status
                    );
                }
            }

            return _mapper.Map<CaseDto>(existingCase);
        }


        public async Task<IEnumerable<CaseDto>> SearchAsync(string? keyword)
        {
            var query = _context.Cases.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(c =>
                    c.Name.Contains(keyword) ||
                    c.Description!.Contains(keyword));

            var results = await query.Include(c => c.CreatedByUser).ToListAsync();
            return _mapper.Map<IEnumerable<CaseDto>>(results);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Cases.FindAsync(id);
            if (entity == null) return false;

            _context.Cases.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        // 🔹 Get All Assignees
        public async Task<IEnumerable<CaseAssigneeDto>> GetAssigneesByCaseIdAsync(int caseId)
        {
            var assignees = await _context.CaseAssignees
                .Include(a => a.User)
                .Where(a => a.CaseId == caseId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CaseAssigneeDto>>(assignees);
        }

        // 🔹 Get All Evidence
        public async Task<IEnumerable<EvidenceDto>> GetEvidenceByCaseIdAsync(int caseId)
        {
            var evidenceList = await _context.Evidence
                .Where(e => e.CaseId == caseId && !e.IsDeleted)
                .ToListAsync();

            return _mapper.Map<IEnumerable<EvidenceDto>>(evidenceList);
        }

        // 🔹 Get All Suspects
        public async Task<IEnumerable<ParticipantDto>> GetSuspectsByCaseIdAsync(int caseId)
        {
            var suspects = await _context.CaseParticipants
                .Include(cp => cp.Participant)
                .Where(cp => cp.CaseId == caseId && cp.Participant.Role == "Suspect")
                .Select(cp => cp.Participant)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ParticipantDto>>(suspects);
        }

        // 🔹 Get All Victims
        public async Task<IEnumerable<ParticipantDto>> GetVictimsByCaseIdAsync(int caseId)
        {
            var victims = await _context.CaseParticipants
                .Include(cp => cp.Participant)
                .Where(cp => cp.CaseId == caseId && cp.Participant.Role == "Victim")
                .Select(cp => cp.Participant)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ParticipantDto>>(victims);
        }

        // 🔹 Get All Witnesses
        public async Task<IEnumerable<ParticipantDto>> GetWitnessesByCaseIdAsync(int caseId)
        {
            var witnesses = await _context.CaseParticipants
                .Include(cp => cp.Participant)
                .Where(cp => cp.CaseId == caseId && cp.Participant.Role == "Witness")
                .Select(cp => cp.Participant)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ParticipantDto>>(witnesses);
        }

        public async Task<object?> GetCaseDetailsAsync(int caseId)
        {
            var caseEntity = await _context.Cases
                .Include(c => c.Assignees)
                .Include(c => c.EvidenceList)
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == caseId);

            if (caseEntity == null) return null;

            var suspects = caseEntity.Participants?.Count(p => p.Participant.Role == "Suspect") ?? 0;
            var victims = caseEntity.Participants?.Count(p => p.Participant.Role == "Victim") ?? 0;
            var witnesses = caseEntity.Participants?.Count(p => p.Participant.Role == "Witness") ?? 0;

            return new
            {
                CaseId = caseEntity.Id,
                caseEntity.Name,
                caseEntity.Status,
                AssigneeCount = caseEntity.Assignees?.Count ?? 0,
                EvidenceCount = caseEntity.EvidenceList?.Count ?? 0,
                SuspectCount = suspects,
                VictimCount = victims,
                WitnessCount = witnesses
            };
        }
    }
}