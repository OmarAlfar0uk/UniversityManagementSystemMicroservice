namespace ProgressService.Data.Models;

public class LectureProgress
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid LectureId { get; set; }
    public Guid CourseId { get; set; }
    public bool IsPdfViewed { get; set; }
    public bool IsVideoWatched { get; set; }
    public bool IsAttendanceRegistered { get; set; }
    public bool IsAssignmentSubmitted { get; set; }
    public bool IsQuizCompleted { get; set; }
    public decimal CompletionPercentage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
