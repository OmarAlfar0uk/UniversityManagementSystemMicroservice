namespace ExamService.Data.Models;

public class QuizAnswer
{
    public Guid Id { get; set; }
    public Guid QuizAttemptId { get; set; }
    public Guid QuizQuestionId { get; set; }
    public string? AnswerText { get; set; }
    public Guid? SelectedOptionId { get; set; }
    public bool? IsCorrect { get; set; }
    public decimal? EarnedPoints { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public QuizAttempt QuizAttempt { get; set; } = default!;
    public QuizQuestion QuizQuestion { get; set; } = default!;
}
