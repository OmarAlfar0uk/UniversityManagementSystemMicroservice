namespace ProgressService.Features.Progress.GetCourseProgress;

public record CourseProgressResponse(
    Guid CourseId,
    Guid StudentId,
    int CompletedLectures,
    decimal CompletionPercentage
);
