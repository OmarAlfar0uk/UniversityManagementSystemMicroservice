using MediatR;

namespace AttendanceService.Features.Attendance.RegisterAttendance;

public record RegisterAttendanceCommand(Guid LectureId, Guid StudentId, string Code) : IRequest;
