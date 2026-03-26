namespace ReportingDashboardService.Dtos
{
    // From AcademicService
    public record CourseDto(Guid Id, string Name, string? CoverImageUrl, Guid DoctorId, int EnrolledCount);
    public record LectureDto(Guid Id, string Title, int OrderIndex);

    // From AttendanceService
    public record CourseAttendanceDto(Guid CourseId, int TotalLectures, int AttendedCount, double Percentage);

    // From GradeService
    public record CourseGradeDto(Guid CourseId, decimal? MidtermScore, decimal? FinalScore, decimal? TotalScore);
    public record StudentGradeDto(Guid StudentId, string StudentName, decimal? MidtermScore, decimal? FinalScore);

    // From ExamService
    public record QuizResultDto(Guid LectureId, decimal Score, bool IsPassed);
    public record QuizStatsDto(Guid QuizId, double AverageScore, int TotalAttempts, int PassedCount);

    // From AuthService
    public record UserInfoDto(Guid Id, string FullName, string Email, string Role);
    public record StudentDto(Guid Id, string FullName, string UniversityId, Guid DepartmentId);
}
