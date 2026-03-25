namespace AuthService.Features.Auth.Admin.GetStudentsByDepartment;

public record DepartmentStudentDto(
    Guid StudentId,
    string Email,
    string FullName,
    string UniversityId,
    Guid? DepartmentId
);
