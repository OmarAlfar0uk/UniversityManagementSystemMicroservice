using Auth_Service.Features.Shared;
using MediatR;

namespace AuthService.Features.Auth.ForgotPassword
{
    public class ForgotPasswordCommand : IRequest<EndpointResponse<ForgotPasswordResponse>>
    {
        public string EmailOrId { get; set; } = default!;
    }
}
