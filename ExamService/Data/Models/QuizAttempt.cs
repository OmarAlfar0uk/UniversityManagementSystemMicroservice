namespace ExamService.Data.Models;

public class QuizAttempt
{
    public Guid Id { get; set; }
    public Guid QuizId { get; set; }
    public Guid StudentId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public decimal? Score { get; set; }
    public bool? IsPassed { get; set; }
    public int? TimeTakenInMinutes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Quiz Quiz { get; set; } = default!;
    public ICollection<QuizAnswer> Answers { get; set; } = [];
}
