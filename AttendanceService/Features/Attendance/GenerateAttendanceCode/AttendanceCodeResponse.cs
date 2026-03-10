namespace AttendanceService.Features.Attendance.GenerateAttendanceCode;

public record AttendanceCodeResponse(string Code, DateTime ExpiresAt, Guid LectureId);
