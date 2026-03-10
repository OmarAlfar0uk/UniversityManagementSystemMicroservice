using AttendanceService.Contracts;
using AttendanceService.Data.Models;
using MediatR;

namespace AttendanceService.Features.Attendance.ManualAttendance;

public class ManualAttendanceHandler : IRequestHandler<ManualAttendanceCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public ManualAttendanceHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ManualAttendanceCommand request, CancellationToken cancellationToken)
    {
        var existing = await _unitOfWork.AttendanceRecords.FindAsync(
            r => r.LectureId == request.LectureId && r.StudentId == request.StudentId);

        if (existing is not null)
        {
            existing.IsAttended = request.IsAttended;
            existing.IsManual = true;
            existing.RegisteredAt = DateTime.UtcNow;
            existing.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.AttendanceRecords.Update(existing);
        }
        else
        {
            var record = new AttendanceRecord
            {
                Id = Guid.NewGuid(),
                StudentId = request.StudentId,
                LectureId = request.LectureId,
                CourseId = Guid.Empty, // requires CourseId lookup in real implementation
                LectureTitle = string.Empty,
                IsAttended = request.IsAttended,
                IsManual = true,
                RegisteredAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _unitOfWork.AttendanceRecords.AddAsync(record);
        }

        await _unitOfWork.SaveChangesAsync();
    }
}
