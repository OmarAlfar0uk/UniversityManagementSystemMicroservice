using ReportingDashboardService.Dtos;

namespace ReportingDashboardService.Contracts
{
    public interface IExamServiceClient
    {
        Task<List<QuizResultDto>> GetStudentQuizResultsAsync(Guid studentId);
        Task<int> GetPendingEssaysCountAsync(Guid doctorId);
        Task<List<QuizStatsDto>> GetCourseQuizStatsAsync(Guid courseId);
    }
}
