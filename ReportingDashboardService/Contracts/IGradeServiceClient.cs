using ReportingDashboardService.Dtos;

namespace ReportingDashboardService.Contracts
{
    public interface IGradeServiceClient
    {
        Task<List<CourseGradeDto>> GetStudentGradesAsync(Guid studentId);
        Task<double?> GetStudentGpaAsync(Guid studentId);
        Task<List<StudentGradeDto>> GetCourseGradesAsync(Guid courseId);
        Task<double> GetCourseAverageGradeAsync(Guid courseId);
    }
}
