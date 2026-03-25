using MediatR;

namespace AcademicService.Features.Departments;

public record CreateDepartmentCommand(string Name, string Code) : IRequest<DepartmentResponse>;
