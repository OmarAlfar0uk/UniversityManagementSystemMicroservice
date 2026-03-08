using System.Linq.Expressions;
using AcademicService.Contracts;
using AcademicService.Data;
using Microsoft.EntityFrameworkCore;

namespace AcademicService.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly AcademicDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AcademicDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id) =>
        await _dbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() =>
        await _dbSet.ToListAsync();

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate) =>
        await _dbSet.Where(predicate).ToListAsync();

    public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate) =>
        await _dbSet.FirstOrDefaultAsync(predicate);

    public async Task AddAsync(T entity) =>
        await _dbSet.AddAsync(entity);

    public async Task AddRangeAsync(IEnumerable<T> entities) =>
        await _dbSet.AddRangeAsync(entities);

    public void Update(T entity) =>
        _context.Entry(entity).State = EntityState.Modified;

    public void Remove(T entity) =>
        _dbSet.Remove(entity);

    public void RemoveRange(IEnumerable<T> entities) =>
        _dbSet.RemoveRange(entities);

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) =>
        await _dbSet.AnyAsync(predicate);

    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate) =>
        await _dbSet.CountAsync(predicate);
}
