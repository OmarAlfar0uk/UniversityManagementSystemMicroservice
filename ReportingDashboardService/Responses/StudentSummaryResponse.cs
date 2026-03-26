namespace ReportingDashboardService.Responses
{
    public record StudentSummaryResponse(
        int TotalEnrolledCourses,
        double OverallAttendancePercentage,
        double? GPA,
        int TotalQuizzesTaken,
        int QuizzesPassed,
        List<CourseSummaryItem> Courses
    );

    public record CourseSummaryItem(
        Guid CourseId,
        string CourseName,
        string? CoverImageUrl,
        int AttendedLectures,
        int TotalLectures,
        double AttendancePercentage,
        decimal? MidtermScore,
        decimal? FinalScore,
        decimal? TotalScore,
        string PerformanceLabel
    );
}
