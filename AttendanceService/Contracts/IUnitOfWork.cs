using AttendanceService.Data.Models;

namespace AttendanceService.Contracts;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<AttendanceCode> AttendanceCodes { get; }
    IGenericRepository<AttendanceRecord> AttendanceRecords { get; }
    Task<int> SaveChangesAsync();
}
