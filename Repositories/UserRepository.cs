using CrimeManagementApi.Data;
using CrimeManagementApi.Models;
using CrimeManagementApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrimeManagementApi.Repositories.Implementations
{
    /// <summary>User repo implementation.</summary>
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context) : base(context) => _context = context;

        public async Task<User?> GetByUsernameAsync(string username)
            => await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

        public async Task<IEnumerable<User>> GetByRoleAsync(string role)
            => await _context.Users.Where(u => u.Role.ToLower() == role.ToLower())
                                   .AsNoTracking().ToListAsync();
    }
}
