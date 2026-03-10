using Auth_Service.Features.Shared;
using MediatR;

namespace AuthService.Features.Auth.Parent.GenerateCode
{
    public class GenerateParentCodeCommand : IRequest<EndpointResponse<GenerateParentCodeResponse>>
    {
    }
}
