namespace AcademicService.Dtos
{
    public sealed class UserInfoDto
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Department { get; set; }
        public Guid? DepartmentId { get; set; }
    }
}
