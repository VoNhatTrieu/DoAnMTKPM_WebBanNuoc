using System.Linq.Expressions;
using apii.Models;

namespace apii.Repositories;

/// <summary>
/// Repository Pattern - Interface
/// </summary>
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    
    // Ownership-based query methods for data segregation
    /// <summary>
    /// Gets entities filtered by owner. If ownerId is null (admin), returns all.
    /// </summary>
    Task<IEnumerable<T>> GetByOwnerAsync(int? ownerId);
    
    /// <summary>
    /// Finds entities matching predicate and owned by specified user
    /// </summary>
    Task<IEnumerable<T>> FindByOwnerAsync(int ownerId, Expression<Func<T, bool>> predicate);
    
    /// <summary>
    /// Gets entity by ID and validates ownership
    /// </summary>
    Task<T?> GetByIdAndOwnerAsync(int id, int ownerId);
}

