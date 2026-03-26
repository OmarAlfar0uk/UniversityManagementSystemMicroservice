using MediatR;

namespace ReportingDashboardService.Features.Doctor.GetDoctorDashboard
{
    public record GetDoctorDashboardQuery(Guid DoctorId) : IRequest<DoctorDashboardResponse>;

    public record DoctorDashboardResponse(
        int TotalCourses,
        int TotalStudents,
        double OverallAttendanceAverage,
        int PendingEssaysToGrade,
        List<DoctorCourseStats> Courses
    );

    public record DoctorCourseStats(
        Guid CourseId,
        string CourseName,
        string? CoverImageUrl,
        int EnrolledStudents,
        double AttendanceAverage
    );
}
