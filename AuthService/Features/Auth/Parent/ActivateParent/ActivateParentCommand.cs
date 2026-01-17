using Auth_Service.Features.Shared;
using MediatR;

namespace AuthService.Features.Auth.Parent.ActivateParent
{
    public class ActivateParentCommand : IRequest<EndpointResponse<ActivateParentResponse>>
    {
        public string Code { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
