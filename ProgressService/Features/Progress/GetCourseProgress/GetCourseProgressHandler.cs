using ProgressService.Contracts;
using MediatR;

namespace ProgressService.Features.Progress.GetCourseProgress;

public class GetCourseProgressHandler : IRequestHandler<GetCourseProgressQuery, CourseProgressResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCourseProgressHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CourseProgressResponse> Handle(GetCourseProgressQuery request, CancellationToken cancellationToken)
    {
        var progress = await _unitOfWork.CourseProgresses.FindAsync(
            p => p.CourseId == request.CourseId && p.StudentId == request.StudentId);

        if (progress is null)
            return new CourseProgressResponse(request.CourseId, request.StudentId, 0, 0m);

        return new CourseProgressResponse(
            progress.CourseId,
            progress.StudentId,
            progress.CompletedLectures,
            progress.CompletionPercentage
        );
    }
}
