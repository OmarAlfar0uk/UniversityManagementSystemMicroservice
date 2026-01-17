using Auth_Service.Features.Shared;
using MediatR;

namespace AuthService.Features.Auth.Parent.GetChildren
{
    public class GetParentChildrenQuery : IRequest<EndpointResponse<List<ParentChildDto>>>
    {
    }
}
