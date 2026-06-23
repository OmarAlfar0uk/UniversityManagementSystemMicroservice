using Auth_Service.Features.Shared;
using MediatR;

namespace AuthService.Features.Parent.GetChildProfile
{
    public record GetChildProfileQuery(Guid ParentId)
        : IRequest<EndpointResponse<ChildProfileResponse>>;
}
