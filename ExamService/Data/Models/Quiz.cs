namespace ExamService.Data.Models;

public class Quiz
{
    public Guid Id { get; set; }
    public Guid LectureId { get; set; }
    public Guid CourseId { get; set; }
    public int TimeLimitInMinutes { get; set; }
    public int MaxAttempts { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<QuizQuestion> Questions { get; set; } = [];
    public ICollection<QuizAttempt> Attempts { get; set; } = [];
}
