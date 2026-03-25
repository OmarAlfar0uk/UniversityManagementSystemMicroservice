using MediatR;

namespace AcademicService.Features.Departments;

public record SyncDepartmentEnrollmentsCommand(Guid DepartmentId)
    : IRequest<SyncDepartmentEnrollmentsResponse>;
