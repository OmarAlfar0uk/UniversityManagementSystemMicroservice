namespace AcademicService.Features.Assignments.GetAssignmentStatus;

public record AssignmentStatusResponse(bool IsSubmitted, DateTime? SubmittedAt);
