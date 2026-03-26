using ReportingDashboardService.Dtos;

namespace ReportingDashboardService.Contracts
{
    public interface IAuthServiceClient
    {
        Task<UserInfoDto?> GetUserInfoAsync(Guid userId);
        Task<List<StudentDto>> GetStudentsByDepartmentAsync(Guid departmentId);
        Task<int> GetTotalUsersCountAsync(string role);
    }
}
