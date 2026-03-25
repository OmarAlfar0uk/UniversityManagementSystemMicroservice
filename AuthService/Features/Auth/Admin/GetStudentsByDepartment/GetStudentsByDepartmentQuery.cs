using Auth_Service.Features.Shared;
using MediatR;

namespace AuthService.Features.Auth.Admin.GetStudentsByDepartment;

public record GetStudentsByDepartmentQuery(Guid DepartmentId)
    : IRequest<EndpointResponse<IReadOnlyList<DepartmentStudentDto>>>;
