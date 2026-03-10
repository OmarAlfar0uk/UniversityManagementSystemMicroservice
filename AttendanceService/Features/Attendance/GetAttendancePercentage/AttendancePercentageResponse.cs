namespace AttendanceService.Features.Attendance.GetAttendancePercentage;

public record AttendancePercentageResponse(Guid CourseId, decimal Percentage);
