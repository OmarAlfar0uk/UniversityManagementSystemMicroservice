using AcademicService.Dtos;

namespace AcademicService.Contracts
{
    public interface IAuthServiceClient
    {
        Task<UserInfoDto?> GetUserInfoAsync(Guid userId);
    }
}
