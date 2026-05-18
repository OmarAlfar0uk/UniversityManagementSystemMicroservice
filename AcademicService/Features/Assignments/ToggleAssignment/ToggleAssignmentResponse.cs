namespace AcademicService.Features.Assignments.ToggleAssignment;

public record ToggleAssignmentResponse(Guid AssignmentId, bool IsOpen);
