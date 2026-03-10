using AttendanceService.Contracts;
using AttendanceService.Features.Attendance.GetCoursesAttendance;
using MediatR;

namespace AttendanceService.Features.Attendance.GetChildAttendance;

public record GetChildAttendanceQuery(Guid StudentId) : IRequest<IEnumerable<CourseAttendanceResponse>>;
