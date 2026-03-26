using AuthService.Features.Auth.Student.GetProfile;
using MediatR;

namespace AuthService.Features.Auth.Student.UpdateProfile;

public record UpdateProfileCommand(
    Guid       StudentId,      // from JWT claims
    string?    FullName,
    string?    PhoneNumber,
    IFormFile? ProfileImage    // nullable — only update if provided
) : IRequest<StudentProfileResponse>;
