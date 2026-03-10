namespace AttendanceService.Features.Attendance.GetLectureAttendees;

public record AttendeeResponse(Guid StudentId, DateTime? RegisteredAt, bool IsManual);
