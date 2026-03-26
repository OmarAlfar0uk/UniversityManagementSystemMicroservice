using MediatR;

namespace ReportingDashboardService.Features.Admin.GetAdminDashboard
{
    public record GetAdminDashboardQuery() : IRequest<AdminDashboardResponse>;

    public record AdminDashboardResponse(
        int TotalStudents,
        int TotalDoctors,
        int TotalCourses,
        double SystemAttendanceAverage,
        List<CourseEnrollmentStat> TopCoursesByEnrollment,
        List<CourseAttendanceStat> LowestAttendanceCourses
    );

    public record CourseEnrollmentStat(
        Guid CourseId,
        string CourseName,
        int EnrolledStudents
    );

    public record CourseAttendanceStat(
        Guid CourseId,
        string CourseName,
        double AttendancePercentage
    );
}
