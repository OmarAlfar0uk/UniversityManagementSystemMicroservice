using Auth_Service.Features.Shared;
using MediatR;

namespace AuthService.Features.Auth.Refresh
{
    public class RefreshTokenCommand : IRequest<EndpointResponse<RefreshTokenResponse>>
    {
        public string RefreshToken { get; set; } = default!;
    }
}
