using MediatR;
using AuthService.Features.Auth.Admin.GetProfile;

namespace AuthService.Features.Auth.Admin.UpdateProfile;

public record UpdateProfileCommand(
    Guid       AdminId,        // from JWT claims
    string?    FullName,
    string?    PhoneNumber,
    IFormFile? ProfileImage    // nullable — only update if provided
) : IRequest<AdminProfileResponse>;
