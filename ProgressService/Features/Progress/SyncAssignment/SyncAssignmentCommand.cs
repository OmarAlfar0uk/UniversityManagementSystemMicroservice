using MediatR;

namespace ProgressService.Features.Progress.SyncAssignment;

public record SyncAssignmentCommand(Guid LectureId, Guid CourseId, Guid StudentId) : IRequest<ProgressResponse>;
