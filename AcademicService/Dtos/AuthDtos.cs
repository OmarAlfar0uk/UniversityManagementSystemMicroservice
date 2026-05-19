namespace AcademicService.Dtos
{
    public sealed class UserInfoDto
    {
        public Guid Id { get; init; }
        public string? FirstName { get; init; }
        public string? FullName { get; init; }
        public string? Email { get; init; }
        public string? Role { get; init; }
        public string? ProfileImageUrl { get; init; }
        public string? Department { get; init; }
        public Guid? DepartmentId { get; init; }
    }
}
