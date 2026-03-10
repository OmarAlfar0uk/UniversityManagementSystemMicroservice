namespace AcademicService.Features.Assignments.GetAssignment;

public record AssignmentResponse(Guid Id, string Title, string FileUrl, Guid LectureId);
