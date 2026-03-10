namespace AttendanceService.Features.Attendance.GetLecturesAttendance;

public record LectureAttendanceResponse(
    Guid LectureId,
    string LectureTitle,
    bool IsAttended,
    DateTime? RegisteredAt
);
