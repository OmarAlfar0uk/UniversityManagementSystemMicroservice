using ProgressService.Contracts;
using ProgressService.Data;
using ProgressService.Data.Models;

namespace ProgressService.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ProgressDbContext _context;

    private IGenericRepository<LectureProgress>? _lectureProgresses;

    public UnitOfWork(ProgressDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<LectureProgress> LectureProgresses =>
        _lectureProgresses ??= new GenericRepository<LectureProgress>(_context);

    public async Task<int> SaveChangesAsync() =>
        await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
