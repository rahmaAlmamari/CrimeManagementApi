using System.Security.Claims;
using CrimeManagementApi.Data;
using CrimeManagementApi.DTOs;
using CrimeManagementApi.Models;
using CrimeManagementApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrimeManagementApi.Services
{
    /// <summary>
    /// Service for managing Case-Report links using composite keys (CaseId + ReportId).
    /// </summary>
    public class CaseReportService : ICaseReportService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CaseReportService> _logger;
        private readonly IHttpContextAccessor _httpContext;

        public CaseReportService(AppDbContext context, ILogger<CaseReportService> logger, IHttpContextAccessor httpContext)
        {
            _context = context;
            _logger = logger;
            _httpContext = httpContext;
        }

        // ==========================================================
        // 🔹 Utility: Get Current User Info
        // ==========================================================
        private string? GetUserRole() => _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
        private int? GetUserId()
        {
            var id = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(id, out var result) ? result : null;
        }

        // ==========================================================
        // 🔹 Get All Links
        // ==========================================================
        public async Task<IEnumerable<CaseReportDto>> GetAllAsync()
        {
            var role = GetUserRole();
            var userId = GetUserId();

            IQueryable<CaseReport> query = _context.CaseReports
                .Include(cr => cr.Case)
                .Include(cr => cr.Report);

            // 🔸 Role-based filtering
            if (role == "Officer" || role == "Investigator")
            {
                query = query.Where(cr =>
                    cr.Report.ReportedByUserId == userId ||
                    cr.Case.Assignees.Any(a => a.UserId == userId));
            }
            else if (role == "Citizen")
            {
                throw new UnauthorizedAccessException("Citizens cannot view case-report links.");
            }

            var result = await query
                .Select(cr => new CaseReportDto
                {
                    CaseId = cr.CaseId,
                    ReportId = cr.ReportId,
                    CaseName = cr.Case.Name,
                    ReportTitle = cr.Report.Title,
                    AreaCity = cr.Report.AreaCity,
                    LinkedAt = cr.LinkedAt,
                    Notes = cr.Notes
                })
                .ToListAsync();

            return result;
        }

        // ==========================================================
        // 🔹 Get by Composite Key (CaseId + ReportId)
        // ==========================================================
        public async Task<CaseReportDto?> GetByIdsAsync(int caseId, int reportId)
        {
            var entity = await _context.CaseReports
                .Include(cr => cr.Case)
                .Include(cr => cr.Report)
                .FirstOrDefaultAsync(cr => cr.CaseId == caseId && cr.ReportId == reportId);

            if (entity == null) return null;

            return new CaseReportDto
            {
                CaseId = entity.CaseId,
                ReportId = entity.ReportId,
                CaseName = entity.Case.Name,
                ReportTitle = entity.Report.Title,
                AreaCity = entity.Report.AreaCity,
                LinkedAt = entity.LinkedAt,
                Notes = entity.Notes
            };
        }

        // ==========================================================
        // 🔹 Create Link
        // ==========================================================
        public async Task<CaseReportDto?> LinkAsync(CreateCaseReportDto dto)
        {
            var role = GetUserRole();
            var userId = GetUserId();

            if (role == "Citizen")
                throw new UnauthorizedAccessException("Citizens cannot create links.");

            var caseEntity = await _context.Cases
                .Include(c => c.Assignees)
                .FirstOrDefaultAsync(c => c.Id == dto.CaseId);
            var reportEntity = await _context.CrimeReports.FirstOrDefaultAsync(r => r.Id == dto.ReportId);

            if (caseEntity == null || reportEntity == null)
                return null;

            // Prevent duplicates
            if (await _context.CaseReports.AnyAsync(cr => cr.CaseId == dto.CaseId && cr.ReportId == dto.ReportId))
                return null;

            // 🔸 Role restrictions
            if (role != "Admin" &&
                !(caseEntity.Assignees.Any(a => a.UserId == userId) || reportEntity.ReportedByUserId == userId))
                throw new UnauthorizedAccessException("You are not authorized to link this case and report.");

            var link = new CaseReport
            {
                CaseId = dto.CaseId,
                ReportId = dto.ReportId,
                LinkedAt = DateTime.UtcNow,
                Notes = dto.Notes
            };

            _context.CaseReports.Add(link);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} linked Case {CaseId} with Report {ReportId}.", userId, dto.CaseId, dto.ReportId);

            return await GetByIdsAsync(dto.CaseId, dto.ReportId);
        }

        // ==========================================================
        // 🔹 Update Link
        // ==========================================================
        public async Task<CaseReportDto?> UpdateAsync(int caseId, int reportId, UpdateCaseReportDto dto)
        {
            var role = GetUserRole();
            if (role != "Admin")
                throw new UnauthorizedAccessException("Only admins can update case-report links.");

            var entity = await _context.CaseReports
                .FirstOrDefaultAsync(cr => cr.CaseId == caseId && cr.ReportId == reportId);

            if (entity == null)
                return null;

            if (dto.CaseId.HasValue) entity.CaseId = dto.CaseId.Value;
            if (dto.ReportId.HasValue) entity.ReportId = dto.ReportId.Value;
            if (!string.IsNullOrWhiteSpace(dto.Notes)) entity.Notes = dto.Notes;
            entity.LinkedAt = dto.UpdatedAt ?? DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("CaseReport link ({CaseId},{ReportId}) updated by admin.", caseId, reportId);

            return await GetByIdsAsync(entity.CaseId, entity.ReportId);
        }

        // ==========================================================
        // 🔹 Unlink (Delete)
        // ==========================================================
        public async Task<bool> UnlinkAsync(int caseId, int reportId)
        {
            var role = GetUserRole();
            var userId = GetUserId();

            var link = await _context.CaseReports
                .Include(cr => cr.Case).ThenInclude(c => c.Assignees)
                .Include(cr => cr.Report)
                .FirstOrDefaultAsync(cr => cr.CaseId == caseId && cr.ReportId == reportId);

            if (link == null)
                return false;

            if (role == "Citizen")
                throw new UnauthorizedAccessException("Citizens cannot unlink case-report links.");

            if (role != "Admin" &&
                !(link.Case.Assignees.Any(a => a.UserId == userId) || link.Report.ReportedByUserId == userId))
                throw new UnauthorizedAccessException("You are not authorized to unlink this case and report.");

            _context.CaseReports.Remove(link);
            await _context.SaveChangesAsync();

            _logger.LogInformation("CaseReport link ({CaseId},{ReportId}) removed by User {UserId}.", caseId, reportId, userId);

            return true;
        }
    }
}
