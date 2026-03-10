using ProgressService.Contracts;
using MediatR;

namespace ProgressService.Features.Progress.GetCourseProgress;

public record GetCourseProgressQuery(Guid CourseId, Guid StudentId) : IRequest<CourseProgressResponse>;
