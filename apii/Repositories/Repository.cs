using Microsoft.EntityFrameworkCore;
using apii.Data;
using apii.Models;
using System.Linq.Expressions;

namespace apii.Repositories;

/// <summary>
/// Generic Repository Pattern Implementation
/// </summary>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate);
    }

    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        if (predicate == null)
            return await _dbSet.CountAsync();
        return await _dbSet.CountAsync(predicate);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }
    
    // Ownership-based methods for data segregation
    public virtual async Task<IEnumerable<T>> GetByOwnerAsync(int? ownerId)
    {
        if (!typeof(IOwnable).IsAssignableFrom(typeof(T)))
        {
            throw new InvalidOperationException($"Type {typeof(T).Name} does not implement IOwnable");
        }
        
        if (ownerId == null)
        {
            // Admin - return all
            return await _dbSet.ToListAsync();
        }
        
        // Regular user - return only owned entities
        return await _dbSet.Where(e => ((IOwnable)e).OwnerId == ownerId.Value).ToListAsync();
    }
    
    public virtual async Task<IEnumerable<T>> FindByOwnerAsync(int ownerId, Expression<Func<T, bool>> predicate)
    {
        if (!typeof(IOwnable).IsAssignableFrom(typeof(T)))
        {
            throw new InvalidOperationException($"Type {typeof(T).Name} does not implement IOwnable");
        }
        
        return await _dbSet
            .Where(e => ((IOwnable)e).OwnerId == ownerId)
            .Where(predicate)
            .ToListAsync();
    }
    
    public virtual async Task<T?> GetByIdAndOwnerAsync(int id, int ownerId)
    {
        if (!typeof(IOwnable).IsAssignableFrom(typeof(T)))
        {
            throw new InvalidOperationException($"Type {typeof(T).Name} does not implement IOwnable");
        }
        
        return await _dbSet
            .Where(e => EF.Property<int>(e, "Id") == id && ((IOwnable)e).OwnerId == ownerId)
            .FirstOrDefaultAsync();
    }
}
