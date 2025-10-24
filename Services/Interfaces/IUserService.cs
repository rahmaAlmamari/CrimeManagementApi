using CrimeManagementApi.DTOs;

namespace CrimeManagementApi.Services.Interfaces
{
    /// <summary>
    /// Defines user management business operations.
    /// </summary>
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<UserDto?> CreateAsync(CreateUserDto dto);
        Task<bool> DeleteAsync(int id);
        Task<UserDto?> UpdateAsync(int id, CreateUserDto dto);
        Task<UserDto?> AuthenticateAsync(string username, string password);

    }
}
