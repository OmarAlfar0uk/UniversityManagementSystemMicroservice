namespace AuthService.Features.Auth.Admin.GetProfile;

public record AdminProfileResponse(
    Guid    Id,
    string  FullName,
    string  Email,
    string? ProfileImageUrl,
    string? PhoneNumber,
    string  Role
);
