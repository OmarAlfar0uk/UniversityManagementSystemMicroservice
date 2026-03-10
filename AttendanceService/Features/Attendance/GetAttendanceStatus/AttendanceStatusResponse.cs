namespace AttendanceService.Features.Attendance.GetAttendanceStatus;

public record AttendanceStatusResponse(Guid LectureId, bool IsAttended);
