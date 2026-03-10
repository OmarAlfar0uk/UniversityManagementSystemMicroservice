using Auth_Service.Features.Shared;
using MediatR;

namespace AuthService.Features.Auth.Activate
{
    public class ActivateCommand : IRequest<EndpointResponse<ActivateResponse>>
    {
        public string Code { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
