using MediatR;

namespace AcademicService.Features.Assignments.SubmitAssignmentFile;

public record SubmitAssignmentFileCommand(
    Guid AssignmentOrLectureId,
    Guid StudentId,
    IFormFile File,
    bool ByAssignmentId = false
) : IRequest;
