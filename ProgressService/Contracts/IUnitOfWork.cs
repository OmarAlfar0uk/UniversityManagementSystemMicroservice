using ProgressService.Data.Models;

namespace ProgressService.Contracts;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<LectureProgress> LectureProgresses { get; }
    Task<int> SaveChangesAsync();
}
