using CrimeManagementApi.DTOs;
using CrimeManagementApi.Models;

namespace CrimeManagementApi.Services.Interfaces
{
    /// <summary>
    /// Defines core operations for managing links between Cases and Crime Reports,
    /// using (CaseId, ReportId) as the composite key.
    /// </summary>
    public interface ICaseReportService
    {
        // ==========================================================
        // 🔹 Retrieval
        // ==========================================================

        /// <summary>
        /// Returns all case-report link records visible to the current user.
        /// </summary>
        Task<IEnumerable<CaseReportDto>> GetAllAsync();

        /// <summary>
        /// Returns a specific case-report link based on CaseId and ReportId.
        /// </summary>
        Task<CaseReportDto?> GetByIdsAsync(int caseId, int reportId);


        // ==========================================================
        // 🔹 Create / Update / Delete
        // ==========================================================

        /// <summary>
        /// Creates a new link between a case and a crime report.
        /// </summary>
        Task<CaseReportDto?> LinkAsync(CreateCaseReportDto dto);

        /// <summary>
        /// Updates the existing case-report link (e.g., update notes or reassign).
        /// </summary>
        Task<CaseReportDto?> UpdateAsync(int caseId, int reportId, UpdateCaseReportDto dto);

        /// <summary>
        /// Unlinks (removes) the relationship between a case and a crime report.
        /// </summary>
        Task<bool> UnlinkAsync(int caseId, int reportId);
    }
}
