using CrimeManagementApi.DTOs;

namespace CrimeManagementApi.Services.Interfaces
{
    public interface ICaseParticipantService
    {
        Task<CaseParticipantDto?> LinkAsync(CreateCaseParticipantDto dto);
        Task<IEnumerable<CaseParticipantDto>> GetAllAsync();
        Task<CaseParticipantDto?> GetByIdAsync(int id);
    }
}
