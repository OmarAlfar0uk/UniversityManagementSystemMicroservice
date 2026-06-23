using Auth_Service.Features.Shared;
using MediatR;

namespace AuthService.Features.Parent.GetChildGrades
{
    public record GetChildGradesQuery(Guid ParentId)
        : IRequest<EndpointResponse<ChildGradesResponse>>;
}
