using MediatR;

namespace AuthService.Features.Auth.Admin.GetProfile;

public record GetProfileQuery(Guid AdminId) : IRequest<AdminProfileResponse>;
