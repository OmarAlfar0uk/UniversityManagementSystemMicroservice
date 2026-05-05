namespace AcademicService.Dtos
{
    public record UserInfoDto(
        Guid Id,
        string FullName,
        string Email,
        string Role,
        string? ProfileImageUrl = null,
        string? Department = null
    );
}
