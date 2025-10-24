using CrimeManagementApi.DTOs;

namespace CrimeManagementApi.Services.Interfaces
{
    public interface IEvidenceService
    {
        Task<EvidenceDto> CreateAsync(CreateEvidenceDto dto);
        Task<EvidenceDto?> UploadAsync(UploadEvidenceDto dto);
        Task<EvidenceDto?> GetByIdAsync(int id);
        Task<IEnumerable<EvidenceDto>> GetByCaseIdAsync(int caseId);
        Task<EvidenceDto?> UpdateAsync(int id, CreateEvidenceDto dto);
        Task<bool> SoftDeleteAsync(int id, int actedByUserId);
        Task<object?> AnalyzeTextAsync(int caseId);
        Task<IEnumerable<string>> ExtractLinksAsync(int caseId);

    }
}
