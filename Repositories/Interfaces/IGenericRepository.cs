using System.Linq.Expressions;

namespace CrimeManagementApi.Repositories.Interfaces
{
    /// <summary>Generic repository for async CRUD with optional include support.</summary>
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>Get all records with optional filter and related entities.</summary>
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string includeProperties = "");

        /// <summary>Get one record by ID.</summary>
        Task<T?> GetByIdAsync(int id);

        /// <summary>Add a new entity.</summary>
        Task AddAsync(T entity);

        /// <summary>Update an existing entity.</summary>
        void Update(T entity);

        /// <summary>Delete an entity.</summary>
        void Delete(T entity);

        /// <summary>Save all changes to DB.</summary>
        Task SaveAsync();
    }
}
