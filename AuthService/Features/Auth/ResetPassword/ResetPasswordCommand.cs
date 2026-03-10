using Auth_Service.Features.Shared;
using MediatR;

namespace AuthService.Features.Auth.ResetPassword
{
    public class ResetPasswordCommand : IRequest<EndpointResponse<string>>
    {
        public string EmailOrId { get; set; } = default!;
        public string ResetToken { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
    }
}
