using AttendanceService.Contracts;
using AttendanceService.Data;
using AttendanceService.Data.Models;

namespace AttendanceService.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AttendanceDbContext _context;

    private IGenericRepository<AttendanceCode>? _attendanceCodes;
    private IGenericRepository<AttendanceRecord>? _attendanceRecords;

    public UnitOfWork(AttendanceDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<AttendanceCode> AttendanceCodes =>
        _attendanceCodes ??= new GenericRepository<AttendanceCode>(_context);

    public IGenericRepository<AttendanceRecord> AttendanceRecords =>
        _attendanceRecords ??= new GenericRepository<AttendanceRecord>(_context);

    public async Task<int> SaveChangesAsync() =>
        await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
