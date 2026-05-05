namespace AcademicService.Dtos
{
    public record UserInfoDto(
        Guid Id,
        string FirstName,
        string FullName,
        string Email,
        string Role,
        string? ProfileImageUrl = null,
        string? Department = null
    );
}
