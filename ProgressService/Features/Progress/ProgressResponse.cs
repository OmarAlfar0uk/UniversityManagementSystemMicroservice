namespace ProgressService.Features.Progress;

public record ProgressResponse(
    Guid     Id,
    Guid     StudentId,
    Guid     LectureId,
    Guid     CourseId,
    bool     IsPdfViewed,
    bool     IsVideoWatched,
    bool     IsAttendanceRegistered,
    bool     IsAssignmentSubmitted,
    bool     IsQuizCompleted,
    decimal  CompletionPercentage,
    DateTime UpdatedAt);
