using AutoMapper;
using CrimeManagementApi.Data;
using CrimeManagementApi.DTOs;
using CrimeManagementApi.Models;
using CrimeManagementApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrimeManagementApi.Services
{
    public class ParticipantService : IParticipantService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ParticipantService> _logger;

        public ParticipantService(AppDbContext context, IMapper mapper, ILogger<ParticipantService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // ======================================================
        // 🔹 Get All
        // ======================================================
        public async Task<IEnumerable<ParticipantDto>> GetAllAsync()
        {
            var entities = await _context.Participants
                .AsNoTracking()
                .OrderByDescending(p => p.AddedOn)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ParticipantDto>>(entities);
        }

        // ======================================================
        // 🔹 Get By Id
        // ======================================================
        public async Task<ParticipantDto?> GetByIdAsync(int id)
        {
            var entity = await _context.Participants.FindAsync(id);
            return entity == null ? null : _mapper.Map<ParticipantDto>(entity);
        }

        // ======================================================
        // 🔹 Create
        // ======================================================
        public async Task<ParticipantDto?> CreateAsync(CreateParticipantDto dto)
        {
            try
            {
                var entity = _mapper.Map<Participant>(dto);
                entity.AddedOn = DateTime.UtcNow;

                _context.Participants.Add(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Participant {Name} added successfully.", entity.FullName);
                return _mapper.Map<ParticipantDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating participant.");
                return null;
            }
        }

        // ======================================================
        // 🔹 Update
        // ======================================================
        public async Task<ParticipantDto?> UpdateAsync(int id, UpdateParticipantDto dto)
        {
            var entity = await _context.Participants.FindAsync(id);
            if (entity == null)
                return null;

            _mapper.Map(dto, entity);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Participant {Id} updated.", id);
            return _mapper.Map<ParticipantDto>(entity);
        }

        // ======================================================
        // 🔹 Delete
        // ======================================================
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Participants.FindAsync(id);
            if (entity == null)
                return false;

            _context.Participants.Remove(entity);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Participant {Id} deleted.", id);
            return true;
        }
    }
}
