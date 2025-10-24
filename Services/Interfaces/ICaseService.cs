using CrimeManagementApi.DTOs;

namespace CrimeManagementApi.Services.Interfaces
{
    /// <summary>
    /// Defines business operations for managing criminal cases.
    /// </summary>
    public interface ICaseService
    {
        /// <summary>Retrieve all cases.</summary>
        Task<IEnumerable<CaseDto>> GetAllAsync();

        /// <summary>Retrieve a single case by ID.</summary>
        Task<CaseDto?> GetByIdAsync(int id);

        /// <summary>Create a new case record.</summary>
        Task<CaseDto?> CreateAsync(CreateCaseDto dto);

        /// <summary>Update existing case details.</summary>
        Task<CaseDto?> UpdateAsync(int id, UpdateCaseDto dto);

        /// <summary>Search cases by keyword (name or description).</summary>
        Task<IEnumerable<CaseDto>> SearchAsync(string? keyword);

        /// <summary>Soft delete or permanently remove a case.</summary>
        Task<bool> DeleteAsync(int id);

        Task<IEnumerable<CaseAssigneeDto>> GetAssigneesByCaseIdAsync(int caseId);
        Task<IEnumerable<EvidenceDto>> GetEvidenceByCaseIdAsync(int caseId);
        Task<IEnumerable<ParticipantDto>> GetSuspectsByCaseIdAsync(int caseId);
        Task<IEnumerable<ParticipantDto>> GetVictimsByCaseIdAsync(int caseId);
        Task<IEnumerable<ParticipantDto>> GetWitnessesByCaseIdAsync(int caseId);
        Task<object?> GetCaseDetailsAsync(int caseId);

    }
}
