using AttendanceService.Contracts;
using MediatR;

namespace AttendanceService.Features.Attendance.GetAttendancePercentage;

public record GetAttendancePercentageQuery(Guid CourseId, Guid StudentId) : IRequest<AttendancePercentageResponse>;
