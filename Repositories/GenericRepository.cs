using CrimeManagementApi.Data;
using CrimeManagementApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CrimeManagementApi.Repositories.Implementations
{
    /// <summary>
    /// Generic repository implementation for async CRUD with optional includes.
    /// </summary>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        /// <summary>Get all with optional filter and related data.</summary>
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string includeProperties = "")
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            foreach (var includeProp in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(includeProp.Trim());

            return await query.AsNoTracking().ToListAsync();
        }

        /// <summary>Get one record by ID.</summary>
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>Add a new entity.</summary>
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        /// <summary>Update an entity.</summary>
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        /// <summary>Delete an entity.</summary>
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        /// <summary>Save all pending changes.</summary>
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
