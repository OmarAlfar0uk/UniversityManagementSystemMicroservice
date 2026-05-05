namespace MessageService.Dtos
{
    public record UserInfoDto(
        Guid Id,
        string FullName,
        string Email,
        string Role
    );
}
