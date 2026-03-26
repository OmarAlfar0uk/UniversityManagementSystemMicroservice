using MediatR;

namespace ReportingDashboardService.Features.Parent.GetChildGradesReport
{
    public record GetChildGradesReportQuery(Guid StudentId) : IRequest<ChildGradesReportResponse>;

    public record ChildGradesReportResponse(
        Guid StudentId,
        double? GPA,
        string GpaLabel,
        List<CourseGradeReport> Courses
    );

    public record CourseGradeReport(
        Guid CourseId,
        string CourseName,
        decimal? MidtermScore,
        decimal? FinalScore,
        decimal? TotalScore,
        string Grade
    );
}
