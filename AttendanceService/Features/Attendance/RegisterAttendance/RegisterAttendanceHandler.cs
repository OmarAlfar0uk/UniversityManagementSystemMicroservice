using AttendanceService.Contracts;
using AttendanceService.Data.Models;
using MassTransit;
using MediatR;
using Shered.Events;

namespace AttendanceService.Features.Attendance.RegisterAttendance;

public class RegisterAttendanceHandler : IRequestHandler<RegisterAttendanceCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;

    public RegisterAttendanceHandler(IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint)
    {
        _unitOfWork = unitOfWork;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Handle(RegisterAttendanceCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate code exists, is active, not expired
        var code = await _unitOfWork.AttendanceCodes.FindAsync(
            c => c.Code == request.Code && c.LectureId == request.LectureId);

        if (code is null)
            throw new KeyNotFoundException("Invalid attendance code.");

        if (!code.IsActive)
            throw new InvalidOperationException("Attendance code is no longer active.");

        if (code.ExpiresAt <= DateTime.UtcNow)
            throw new InvalidOperationException("Attendance code has expired.");

        // 2. Check student hasn't already registered
        var alreadyRegistered = await _unitOfWork.AttendanceRecords.AnyAsync(
            r => r.StudentId == request.StudentId && r.LectureId == request.LectureId);

        if (alreadyRegistered)
            throw new InvalidOperationException("Attendance already registered for this lecture.");

        // 3. Create record
        var record = new AttendanceRecord
        {
            Id = Guid.NewGuid(),
            StudentId = request.StudentId,
            LectureId = request.LectureId,
            CourseId = code.CourseId,
            LectureTitle = string.Empty, // cross-service ref — resolved by client
            IsAttended = true,
            IsManual = false,
            RegisteredAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.AttendanceRecords.AddAsync(record);
        await _unitOfWork.SaveChangesAsync();

        // ✅ Publish event to RabbitMQ
        await _publishEndpoint.Publish<IAttendanceRegistered>(new
        {
            StudentId = record.StudentId,
            CourseId  = record.CourseId,
            LectureId = record.LectureId,
            Date      = DateTime.UtcNow
        }, cancellationToken);
    }
}

