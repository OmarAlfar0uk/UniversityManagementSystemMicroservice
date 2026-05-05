using MediatR;

namespace AuthService.Features.Auth.Doctor.UpdateProfile;

public record UpdateProfileCommand(
    Guid       DoctorId,       // from JWT claims
    string?    FullName,
    string?    PhoneNumber,
    IFormFile? ProfileImage    // nullable — only update if provided
) : IRequest<DoctorProfileResponse>;
