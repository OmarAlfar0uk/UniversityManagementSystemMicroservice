using AttendanceService.Contracts;
using MediatR;

namespace AttendanceService.Features.Attendance.GetAttendanceStatus;

public record GetAttendanceStatusQuery(Guid LectureId, Guid StudentId) : IRequest<AttendanceStatusResponse>;
