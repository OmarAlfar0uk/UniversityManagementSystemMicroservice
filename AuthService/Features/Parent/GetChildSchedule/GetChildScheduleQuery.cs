using Auth_Service.Features.Shared;
using MediatR;

namespace AuthService.Features.Parent.GetChildSchedule
{
    public record GetChildScheduleQuery(Guid ParentId)
        : IRequest<EndpointResponse<ChildScheduleResponse>>;
}
