using MediatR;

namespace ProgressService.Features.Progress.MarkPdfViewed;

public record MarkPdfViewedCommand(Guid LectureId, Guid CourseId, Guid StudentId) : IRequest<ProgressResponse>;
