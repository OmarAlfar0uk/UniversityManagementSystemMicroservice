using Auth_Service.Features.Shared;
using MediatR;

namespace AuthService.Features.Auth.Admin.ToggleUserStatus
{
    public class ToggleUserStatusCommand
        : IRequest<EndpointResponse<string>>
    {
        public Guid UserId { get; set; }
        public bool Enable { get; set; } 
    }
}
