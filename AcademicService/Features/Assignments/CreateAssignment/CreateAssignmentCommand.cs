using MediatR;

namespace AcademicService.Features.Assignments.CreateAssignment;

public record CreateAssignmentCommand(
    Guid LectureId,
    Guid CourseId,
    string Title,
    string Instructions,
    DateTime Deadline,
    bool IsOpen
) : IRequest<CreateAssignmentResponse>;
