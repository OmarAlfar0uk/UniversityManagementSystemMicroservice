namespace ReportingDashboardService.Responses
{
    public record CourseStatsResponse(
        Guid CourseId,
        string CourseName,
        int TotalLectures,
        int EnrolledStudents,
        double AttendanceAverage,
        double GradeAverage,
        int PassedStudents,
        int FailedStudents,
        List<QuizStatItem> QuizStats
    );

    public record QuizStatItem(
        Guid QuizId,
        double AverageScore,
        int TotalAttempts,
        int PassedCount,
        double PassRate
    );
}
