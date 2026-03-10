using Auth_Service.Features.Shared;
using MediatR;

namespace AuthService.Features.Auth.Login
{
    public class LoginCommand : IRequest<EndpointResponse<LoginResponse>>
    {
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
        public bool RememberMe { get; set; } = false;
    }
}
