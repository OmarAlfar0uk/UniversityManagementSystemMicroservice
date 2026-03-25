namespace AuthService.Features.Auth.Admin.CreateStudent
{
    public class CreateStudentResponse
    {
        public Guid StudentId { get; set; }
        public string Email { get; set; } = default!;
        public string ActivationCode { get; set; } = default!;
        public string UniversityId { get; set; } = default!;
        public Guid? DepartmentId { get; set; }
    }
}
