namespace ProgressService.Features.Progress.GetLectureProgress;

public record LectureProgressResponse(Guid LectureId, Guid StudentId, bool IsCompleted, DateTime? CompletedAt);
