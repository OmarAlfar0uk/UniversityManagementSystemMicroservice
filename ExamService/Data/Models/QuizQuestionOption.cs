namespace ExamService.Data.Models;

public class QuizQuestionOption
{
    public Guid Id { get; set; }
    public Guid QuizQuestionId { get; set; }
    public string Text { get; set; } = default!;
    public bool IsCorrect { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public QuizQuestion QuizQuestion { get; set; } = default!;
}
