using AttendanceService.Contracts;
using MediatR;

namespace AttendanceService.Features.Attendance.GetCoursesAttendance;

public record GetCoursesAttendanceQuery(Guid StudentId) : IRequest<IEnumerable<CourseAttendanceResponse>>;
