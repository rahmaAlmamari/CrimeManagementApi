using CrimeManagementApi.Data;
using CrimeManagementApi.Models;
using CrimeManagementApi.Services.Interfaces;
using CrimeManagementApi.Utilities;
using Microsoft.EntityFrameworkCore;

namespace CrimeManagementApi.Services
{
    /// <summary>
    /// Handles generation of case reports in PDF format.
    /// </summary>
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Generates and returns a PDF report for a given case.
        /// </summary>
        public async Task<byte[]> GenerateCaseReportAsync(int caseId)
        {
            var caseEntity = await _context.Cases
                .Include(c => c.CreatedByUser)
                .Include(c => c.EvidenceList)
                .FirstOrDefaultAsync(c => c.Id == caseId);

            if (caseEntity == null)
                throw new KeyNotFoundException($"Case with ID {caseId} not found.");

            // Use your PdfGenerator utility
            return PdfGenerator.GenerateCaseReport(caseEntity);
        }
    }
}
