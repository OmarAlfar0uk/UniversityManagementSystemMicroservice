using AttendanceService.Contracts;
using MediatR;

namespace AttendanceService.Features.Attendance.GetLectureAttendees;

public record GetLectureAttendeesQuery(Guid LectureId) : IRequest<IEnumerable<AttendeeResponse>>;
