using MediatR;

namespace AcademicService.Features.Departments.GetDepartmentDetails;

public record GetDepartmentDetailsQuery(Guid DepartmentId)
    : IRequest<DepartmentDetailsResponse>;
