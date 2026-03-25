using MediatR;

namespace AcademicService.Features.Departments;

public record GetDepartmentsQuery() : IRequest<IReadOnlyList<DepartmentResponse>>;
