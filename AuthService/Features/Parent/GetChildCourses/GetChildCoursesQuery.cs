using Auth_Service.Features.Shared;
using MediatR;

namespace AuthService.Features.Parent.GetChildCourses
{
    public record GetChildCoursesQuery(Guid ParentId)
        : IRequest<EndpointResponse<ChildCoursesResponse>>;
}
