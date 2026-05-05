namespace AcademicService.Features.Assignments.GetAssignment;

public record AssignmentResponse(
    Guid Id,
    string Title,
    string Instructions,
    string FileUrl,
    Guid LectureId,
    Guid CourseId,
    DateTime Deadline,
    bool IsOpen
);
