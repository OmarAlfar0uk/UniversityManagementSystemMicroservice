using MediatR;

namespace AcademicService.Features.Assignments.SubmitAssignmentFile;

public record SubmitAssignmentFileCommand(Guid LectureId, Guid StudentId, IFormFile File) : IRequest;
