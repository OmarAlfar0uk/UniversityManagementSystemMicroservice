namespace AuthService.Features.Auth.Doctor.GetProfile;

public record DoctorProfileResponse(
    Guid    Id,
    string  FullName,
    string  Email,
    string? ProfileImageUrl,
    string? PhoneNumber,
    string  Role,
    Guid?   DepartmentId
);
