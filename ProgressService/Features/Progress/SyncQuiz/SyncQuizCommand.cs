using MediatR;

namespace ProgressService.Features.Progress.SyncQuiz;

public record SyncQuizCommand(Guid LectureId, Guid CourseId, Guid StudentId) : IRequest<ProgressResponse>;
