namespace AttendanceService.Features.Attendance.GetCoursesAttendance;

public record CourseAttendanceResponse(
    Guid CourseId,
    int TotalLectures,
    int AttendedCount,
    decimal Percentage
);
