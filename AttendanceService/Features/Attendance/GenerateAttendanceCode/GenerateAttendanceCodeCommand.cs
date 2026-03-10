using AttendanceService.Contracts;
using MediatR;

namespace AttendanceService.Features.Attendance.GenerateAttendanceCode;

public record GenerateAttendanceCodeCommand(
    Guid LectureId,
    Guid CourseId,
    int ExpiresInMinutes
) : IRequest<AttendanceCodeResponse>;
