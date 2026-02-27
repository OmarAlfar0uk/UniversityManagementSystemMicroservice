using Auth_Service.Features.Shared;
using MediatR;

namespace AuthService.Features.Auth.Admin.DeleteUser
{
    public class DeleteUserCommand : IRequest<EndpointResponse<string>>
    {
        public Guid UserId { get; set; }
    }
}
