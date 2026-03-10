using GradeService.Contracts;
using GradeService.Data;
using GradeService.Data.Models;

namespace GradeService.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly GradeDbContext _context;

    private IGenericRepository<StudentGrade>? _studentGrades;

    public UnitOfWork(GradeDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<StudentGrade> StudentGrades =>
        _studentGrades ??= new GenericRepository<StudentGrade>(_context);

    public async Task<int> SaveChangesAsync() =>
        await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
