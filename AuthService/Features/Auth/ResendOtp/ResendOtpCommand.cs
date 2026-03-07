using Auth_Service.Features.Shared;
using AuthService.Features.Auth.ForgotPassword;
using MediatR;

namespace AuthService.Features.Auth.ResendOtp
{
    public class ResendOtpCommand : IRequest<EndpointResponse<ForgotPasswordResponse>>
    {
        public string EmailOrId { get; set; } = default!;
    }
}
