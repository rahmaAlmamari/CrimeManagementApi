using CrimeManagementApi.DTOs;

namespace CrimeManagementApi.Services.Interfaces
{
    /// <summary>
    /// Defines operations for managing case participants (victims, suspects, witnesses, etc.)
    /// </summary>
    public interface IParticipantService
    {
        Task<IEnumerable<ParticipantDto>> GetAllAsync();
        Task<ParticipantDto?> GetByIdAsync(int id);
        Task<ParticipantDto?> CreateAsync(CreateParticipantDto dto);
        Task<ParticipantDto?> UpdateAsync(int id, UpdateParticipantDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
