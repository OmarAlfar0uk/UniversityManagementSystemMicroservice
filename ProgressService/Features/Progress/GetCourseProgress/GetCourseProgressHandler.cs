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
        var allLectureProgresses = await _unitOfWork.LectureProgresses.GetAllAsync(
            p => p.CourseId == request.CourseId && p.StudentId == request.StudentId);

        if (!allLectureProgresses.Any())
            return new CourseProgressResponse(request.CourseId, request.StudentId, 0, 0m);

        var completedLectures = allLectureProgresses.Count(p => p.CompletionPercentage >= 100m);
        // Note: dynamically calculating course CompletionPercentage requires TotalLectures, 
        // which isn't available here. For now returning 0m.
        var completionPercentage = 0m;

        return new CourseProgressResponse(
            request.CourseId,
            request.StudentId,
            completedLectures,
            completionPercentage
        );
    }
}
