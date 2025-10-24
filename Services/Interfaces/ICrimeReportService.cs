using CrimeManagementApi.DTOs;

namespace CrimeManagementApi.Services.Interfaces
{
    /// <summary>
    /// Defines operations for managing citizen and admin crime reports.
    /// </summary>
    public interface ICrimeReportService
    {
        /// <summary>Create a new crime report (citizen/public side).</summary>
        Task<CrimeReportDto?> CreateReportAsync(CreateCrimeReportDto dto);

        /// <summary>Retrieve all reports (admin/investigator view).</summary>
        Task<IEnumerable<CrimeReportDto>> GetAllAsync();

        /// <summary>Retrieve a specific report by ID.</summary>
        Task<CrimeReportDto?> GetByIdAsync(int id);

        /// <summary>Get the current status of a report for tracking.</summary>
        Task<string> GetStatusAsync(int id);
    }
}
