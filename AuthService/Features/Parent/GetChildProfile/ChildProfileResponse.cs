namespace AuthService.Features.Parent.GetChildProfile
{
    public record ChildProfileResponse(
        Guid    StudentId,
        string  FullName,
        string? Email,
        string  UniversityId,
        Guid?   DepartmentId,
        string? ProfileImageUrl
    );
}
