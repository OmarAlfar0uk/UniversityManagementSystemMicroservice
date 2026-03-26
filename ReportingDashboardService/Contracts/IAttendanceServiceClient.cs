using ReportingDashboardService.Dtos;

namespace ReportingDashboardService.Contracts
{
    public interface IAttendanceServiceClient
    {
        Task<List<CourseAttendanceDto>> GetStudentAttendanceAsync(Guid studentId);
        Task<double> GetStudentOverallAttendancePercentageAsync(Guid studentId);
        Task<List<CourseAttendanceDto>> GetCourseAttendanceAsync(Guid courseId);
        Task<double> GetCourseAttendanceAverageAsync(Guid courseId);
    }
}
