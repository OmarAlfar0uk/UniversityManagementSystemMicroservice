using MediatR;

namespace AttendanceService.Features.Attendance.ManualAttendance;

public record ManualAttendanceCommand(Guid LectureId, Guid StudentId, bool IsAttended) : IRequest;
