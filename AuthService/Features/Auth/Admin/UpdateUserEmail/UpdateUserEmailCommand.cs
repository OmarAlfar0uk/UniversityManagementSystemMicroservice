using Auth_Service.Features.Shared;
using MediatR;

namespace AuthService.Features.Auth.Admin.UpdateUserEmail
{
    public class UpdateUserEmailCommand : IRequest<EndpointResponse<string>>
    {
        public Guid UserId { get; set; }
        public string NewEmail { get; set; } = string.Empty;
    }
}
