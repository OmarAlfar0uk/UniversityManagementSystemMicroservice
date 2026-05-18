using MediatR;

namespace AcademicService.Features.Assignments.ToggleAssignment;

public record ToggleAssignmentCommand(Guid AssignmentId) : IRequest<ToggleAssignmentResponse>;
