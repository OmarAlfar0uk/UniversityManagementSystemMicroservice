using MediatR;

namespace AcademicService.Features.Assignments.SubmitAssignmentUrl;

public record SubmitAssignmentUrlCommand(Guid LectureId, Guid StudentId, string ProjectUrl) : IRequest;
