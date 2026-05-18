using MediatR;

namespace AcademicService.Features.Assignments.GetAssignmentStats;

public record GetAssignmentStatsQuery(Guid AssignmentId) : IRequest<AssignmentStatsResponse>;
