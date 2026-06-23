namespace AuthService.Features.Parent.GetChildCourses
{
    public record ChildCoursesResponse(
        Guid                  StudentId,
        string                StudentName,
        List<ChildCourseItem> Courses
    );

    public record ChildCourseItem(
        Guid     CourseId,
        string   CourseName,
        string?  CoverImageUrl,
        Guid     DoctorId,
        decimal? MidtermScore,
        decimal? FinalScore,
        decimal? TotalScore,
        string   PerformanceLabel  // "Excellent" / "Good" / "At Risk" / "No Data"
    );
}
