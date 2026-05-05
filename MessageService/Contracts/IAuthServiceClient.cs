using MessageService.Dtos;

namespace MessageService.Contracts
{
    public interface IAuthServiceClient
    {
        Task<UserInfoDto?> GetUserInfoAsync(Guid userId);
    }
}
