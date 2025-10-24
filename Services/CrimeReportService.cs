using AutoMapper;
using CrimeManagementApi.Data;
using CrimeManagementApi.DTOs;
using CrimeManagementApi.Models;
using CrimeManagementApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrimeManagementApi.Services
{
    /// <summary>Handles citizen and admin crime reports.</summary>
    public class CrimeReportService : ICrimeReportService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public CrimeReportService(AppDbContext context, IMapper mapper, IEmailService emailService)
        {
            _context = context;
            _mapper = mapper;
            _emailService = emailService;
        }


        public async Task<CrimeReportDto?> CreateReportAsync(CreateCrimeReportDto dto)
        {
            var entity = _mapper.Map<CrimeReport>(dto);
            entity.Status = "Pending";
            entity.ReportDateTime = DateTime.UtcNow;

            _context.CrimeReports.Add(entity);
            await _context.SaveChangesAsync();

            //  After saving, send notifications
            try
            {
                // Notify Admins and Investigators about the new report
                var officials = await _context.Users
                    .Where(u => u.Role == "Admin" || u.Role == "Investigator")
                    .ToListAsync();

                foreach (var official in officials)
                {
                    await _emailService.SendNewCrimeAlertAsync(
                        to: official.Email,
                        area: entity.AreaCity ?? "Unknown area",
                        description: $"New crime report submitted: {entity.Title}. Description: {entity.Description}"
                    );
                }

                // Notify Citizens in the same area for awareness
                if (!string.IsNullOrWhiteSpace(entity.AreaCity))
                {
                    var citizens = await _context.Users
                        .Where(u => u.Role == "Citizen")
                        .ToListAsync();

                    foreach (var citizen in citizens)
                    {
                        await _emailService.SendNewCrimeAlertAsync(
                            to: citizen.Email,
                            area: entity.AreaCity,
                            description: $"A new crime incident has been reported near your area: {entity.Title}."
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Email sending failed: {ex.Message}");
            }

            return _mapper.Map<CrimeReportDto>(entity);
        }


        public async Task<IEnumerable<CrimeReportDto>> GetAllAsync()
        {
            var list = await _context.CrimeReports
                .OrderByDescending(r => r.ReportDateTime)
                .ToListAsync();
            return _mapper.Map<IEnumerable<CrimeReportDto>>(list);
        }

        public async Task<CrimeReportDto?> GetByIdAsync(int id)
        {
            var entity = await _context.CrimeReports.FindAsync(id);
            return entity is null ? null : _mapper.Map<CrimeReportDto>(entity);
        }

        public async Task<string> GetStatusAsync(int id)
        {
            var entity = await _context.CrimeReports.FindAsync(id);
            return entity?.Status ?? "Not Found";
        }
    }
}
