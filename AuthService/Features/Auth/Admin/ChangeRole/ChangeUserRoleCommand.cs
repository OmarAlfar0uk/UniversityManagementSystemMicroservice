using Auth_Service.Features.Shared;
using MediatR;

namespace AuthService.Features.Auth.Admin.ChangeRole
{
    public class ChangeUserRoleCommand
       : IRequest<EndpointResponse<string>>
    {
        public Guid UserId { get; set; }
        public string NewRole { get; set; } = default!;
    }
}
