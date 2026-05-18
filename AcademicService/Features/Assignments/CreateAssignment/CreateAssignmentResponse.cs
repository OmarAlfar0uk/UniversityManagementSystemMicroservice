namespace AcademicService.Features.Assignments.CreateAssignment;

public record CreateAssignmentResponse(
    Guid Id,
    Guid LectureId,
    Guid CourseId,
    string Title,
    string Instructions,
    DateTime Deadline,
    bool IsOpen
);
