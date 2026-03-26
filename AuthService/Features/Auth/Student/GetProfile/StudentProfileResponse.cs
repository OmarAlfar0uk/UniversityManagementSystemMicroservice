namespace AuthService.Features.Auth.Student.GetProfile;

public record StudentProfileResponse(
    Guid    Id,
    string  FullName,
    string  Email,
    string  UniversityId,
    Guid?   DepartmentId,
    string  Role,
    string? PhoneNumber,
    string? ProfileImageUrl   // full URL built at response time
);
