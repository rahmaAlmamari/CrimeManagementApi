using CrimeManagementApi.DTOs;

namespace CrimeManagementApi.Services.Interfaces
{
    /// <summary>
    /// Handles linking officers or investigators to cases.
    /// </summary>
    public interface ICaseAssigneeService
    {
        Task<IEnumerable<CaseAssigneeDto>> GetByCaseIdAsync(int caseId);
        Task<CaseAssigneeDto?> AssignAsync(CreateCaseAssigneeDto dto);
        Task<CaseAssigneeDto?> UpdateProgressAsync(int id, string progressStatus);
        Task<bool> RemoveAsync(int id);
    }
}
