using CrimeManagementApi.Models;

namespace CrimeManagementApi.Repositories.Interfaces
{
    /// <summary>User repo interface.</summary>
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<IEnumerable<User>> GetByRoleAsync(string role);
    }
}
