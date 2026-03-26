using MediatR;

namespace ProgressService.Features.Progress.MarkVideoWatched;

public record MarkVideoWatchedCommand(Guid LectureId, Guid CourseId, Guid StudentId) : IRequest<ProgressResponse>;
