using ProgressService.Contracts;
using MediatR;

namespace ProgressService.Features.Progress.GetLectureProgress;

public record GetLectureProgressQuery(Guid LectureId, Guid StudentId) : IRequest<LectureProgressResponse>;
