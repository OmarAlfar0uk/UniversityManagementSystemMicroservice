using GradeService.Data.Models;

namespace GradeService.Contracts;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<StudentGrade> StudentGrades { get; }
    Task<int> SaveChangesAsync();
}
