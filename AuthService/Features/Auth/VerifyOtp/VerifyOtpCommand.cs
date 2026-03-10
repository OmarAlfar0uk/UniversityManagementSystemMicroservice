using Auth_Service.Features.Shared;
using MediatR;

namespace AuthService.Features.Auth.VerifyOtp
{
    public class VerifyOtpCommand : IRequest<EndpointResponse<VerifyOtpResponse>>
    {
        public string EmailOrId { get; set; } = default!;
        public string Otp { get; set; } = default!;
    }
}
