using AttendanceService.Contracts;
using MediatR;

namespace AttendanceService.Features.Attendance.GetLecturesAttendance;

public record GetLecturesAttendanceQuery(Guid CourseId, Guid StudentId) : IRequest<IEnumerable<LectureAttendanceResponse>>;
