using CrimeManagementApi.DTOs;

namespace CrimeManagementApi.Services.Interfaces
{
    /// <summary>Authentication and JWT handling.</summary>
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    }
}
