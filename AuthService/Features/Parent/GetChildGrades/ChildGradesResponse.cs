namespace AuthService.Features.Parent.GetChildGrades
{
    public record ChildGradesResponse(
        Guid                  StudentId,
        string                StudentName,
        double?               GPA,
        string                GpaLabel,   // "Excellent" / "Very Good" / "Good" / "Pass" / "Fail"
        List<CourseGradeItem> Grades
    );

    public record CourseGradeItem(
        Guid     CourseId,
        string?  CourseName,
        decimal? MidtermScore,
        decimal? FinalScore,
        decimal? TotalScore,
        string   GradeLetter   // "A" / "B" / "C" / "D" / "F" / "N/A"
    );
}
