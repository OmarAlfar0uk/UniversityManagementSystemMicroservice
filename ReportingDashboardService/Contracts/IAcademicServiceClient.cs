using ReportingDashboardService.Dtos;

namespace ReportingDashboardService.Contracts
{
    public interface IAcademicServiceClient
    {
        Task<List<CourseDto>> GetEnrolledCoursesAsync(Guid studentId);
        Task<List<CourseDto>> GetDoctorCoursesAsync(Guid doctorId);
        Task<List<CourseDto>> GetAllCoursesAsync();
        Task<int> GetTotalCoursesCountAsync();
        Task<List<LectureDto>> GetCourseLecturesAsync(Guid courseId);
        Task<int> GetCourseEnrollmentCountAsync(Guid courseId);
    }
}
