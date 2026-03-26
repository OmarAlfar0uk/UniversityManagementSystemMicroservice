using MediatR;

namespace ReportingDashboardService.Features.Parent.GetChildAttendanceReport
{
    public record GetChildAttendanceReportQuery(Guid StudentId) : IRequest<ChildAttendanceReportResponse>;

    public record ChildAttendanceReportResponse(
        Guid StudentId,
        double OverallAttendancePercentage,
        List<CourseAttendanceReport> Courses
    );

    public record CourseAttendanceReport(
        Guid CourseId,
        string CourseName,
        int TotalLectures,
        int AttendedLectures,
        double Percentage,
        string StatusLabel
    );
}
