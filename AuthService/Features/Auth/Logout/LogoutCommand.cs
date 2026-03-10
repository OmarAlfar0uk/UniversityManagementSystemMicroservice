using Auth_Service.Features.Shared;
using MediatR;

namespace AuthService.Features.Auth.Logout
{
    public class LogoutCommand : IRequest<EndpointResponse<string>>
    {
        public string RefreshToken { get; set; } = default!;
    }
}
