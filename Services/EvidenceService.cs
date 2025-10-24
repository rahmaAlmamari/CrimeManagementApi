using AutoMapper;
using CrimeManagementApi.Data;
using CrimeManagementApi.DTOs;
using CrimeManagementApi.Models;
using CrimeManagementApi.Repositories.Interfaces;
using CrimeManagementApi.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace CrimeManagementApi.Services.Implementations
{
    public class EvidenceService : IEvidenceService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAuditService _auditService;
        private readonly IWebHostEnvironment _env;
        private readonly IEvidenceRepository _evidenceRepo;

        public EvidenceService(
            AppDbContext context,
            IMapper mapper,
            IAuditService auditService,
            IWebHostEnvironment env,
            IEvidenceRepository evidenceRepo)
        {
            _context = context;
            _mapper = mapper;
            _auditService = auditService;
            _env = env;
            _evidenceRepo = evidenceRepo;
        }

        // ======================================================
        // 🔹 Create Evidence (JSON version)
        // ======================================================
        public async Task<EvidenceDto> CreateAsync(CreateEvidenceDto dto)
        {
            var entity = _mapper.Map<Evidence>(dto);
            entity.AddedAt = DateTime.UtcNow;
            entity.IsDeleted = false;

            await _evidenceRepo.AddAsync(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<EvidenceDto>(entity);
        }

        // ======================================================
        // 🔹 Upload Evidence (multipart/form-data)
        // ======================================================
        public async Task<EvidenceDto?> UploadAsync(UploadEvidenceDto dto)
        {
            try
            {
                if (dto.File == null || dto.File.Length == 0)
                    throw new ArgumentException("No file was provided for upload.");

                // 🔸 Create uploads directory if missing
                string uploadFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "evidence");
                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                // 🔸 Generate safe unique filename
                string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(dto.File.FileName)}";
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                // 🔸 Save file to disk
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.File.CopyToAsync(stream);
                }

                // 🔸 Normalize path for DB storage
                string dbPath = Path.Combine("uploads", "evidence", uniqueFileName)
                                .Replace("\\", "/");

                // 🔸 Create Evidence entity
                var evidence = new Evidence
                {
                    CaseId = dto.CaseId,
                    Type = dto.Type ?? "Document",
                    Description = dto.Description ?? dto.File.FileName,
                    FilePath = dbPath,
                    MimeType = dto.File.ContentType,
                    Remarks = dto.Remarks,
                    AddedAt = DateTime.UtcNow,
                    AddedByUserId = dto.AddedByUserId,
                    IsDeleted = false
                };

                _context.Evidence.Add(evidence);
                await _context.SaveChangesAsync();

                // 🔸 Log upload action
                await _auditService.AddLogAsync(new EvidenceAuditDto
                {
                    EvidenceId = evidence.Id,
                    ActedByUserId = dto.AddedByUserId,
                    Action = "UPLOAD",
                    Details = $"File '{dto.File.FileName}' uploaded and stored at '{dbPath}'",
                    ActedAt = DateTime.UtcNow
                });

                return _mapper.Map<EvidenceDto>(evidence);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UploadAsync] Error: {ex.Message}");
                return null;
            }
        }

        // ======================================================
        // 🔹 Get Evidence by ID
        // ======================================================
        public async Task<EvidenceDto?> GetByIdAsync(int id)
        {
            var evidence = await _context.Evidence
                .Include(e => e.AddedByUser)
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

            return evidence == null ? null : _mapper.Map<EvidenceDto>(evidence);
        }

        // ======================================================
        // 🔹 Get Evidence by Case ID
        // ======================================================
        public async Task<IEnumerable<EvidenceDto>> GetByCaseIdAsync(int caseId)
        {
            var evidences = await _context.Evidence
                .Where(e => e.CaseId == caseId && !e.IsDeleted)
                .ToListAsync();

            return _mapper.Map<IEnumerable<EvidenceDto>>(evidences);
        }

        // ======================================================
        // 🔹 Update Evidence
        // ======================================================
        public async Task<EvidenceDto?> UpdateAsync(int id, CreateEvidenceDto dto)
        {
            var entity = await _context.Evidence.FindAsync(id);
            if (entity == null) return null;

            _mapper.Map(dto, entity);
            _context.Evidence.Update(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<EvidenceDto>(entity);
        }

        // ======================================================
        // 🔹 Soft Delete
        // ======================================================
        public async Task<bool> SoftDeleteAsync(int id, int actedByUserId)
        {
            var entity = await _context.Evidence.FindAsync(id);
            if (entity == null) return false;

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow; 

            _context.Evidence.Update(entity);
            await _context.SaveChangesAsync();

            await _auditService.AddLogAsync(new EvidenceAuditDto
            {
                EvidenceId = id,
                ActedByUserId = actedByUserId,
                Action = "SOFT_DELETE",
                Details = $"Evidence {id} soft-deleted by user {actedByUserId}",
                ActedAt = DateTime.UtcNow
            });

            return true;
        }

        public async Task<object?> AnalyzeTextAsync(int caseId)
        {
            var evidenceList = await _context.Evidence
                .Where(e => e.CaseId == caseId && !string.IsNullOrWhiteSpace(e.Description))
                .ToListAsync();

            if (!evidenceList.Any()) return null;

            var text = string.Join(" ", evidenceList.Select(e => e.Description));
            var words = text.Split(new[] { ' ', ',', '.', ';', ':', '\n', '\r' },
                                   StringSplitOptions.RemoveEmptyEntries);

            var topWords = words
                .GroupBy(w => w.ToLower())
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => new { Word = g.Key, Count = g.Count() });

            return topWords;
        }

        public async Task<IEnumerable<string>> ExtractLinksAsync(int caseId)
        {
            var evidenceList = await _context.Evidence
                .Where(e => e.CaseId == caseId &&
                           (!string.IsNullOrEmpty(e.Description) || !string.IsNullOrEmpty(e.Remarks)))
                .ToListAsync();

            var text = string.Join(" ", evidenceList.Select(e => e.Description + " " + e.Remarks));
            var regex = new Regex(@"https?://[^\s]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matches = regex.Matches(text).Select(m => m.Value).Distinct();

            return matches;
        }


    }
}
