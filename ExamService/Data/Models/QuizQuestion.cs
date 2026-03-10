using ExamService.Data.Enums;

namespace ExamService.Data.Models;

public class QuizQuestion
{
    public Guid Id { get; set; }
    public Guid QuizId { get; set; }
    public string Text { get; set; } = default!;
    public QuestionType Type { get; set; }
    public decimal Points { get; set; }
    public int OrderIndex { get; set; }
    public string? CorrectAnswer { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Quiz Quiz { get; set; } = default!;
    public ICollection<QuizQuestionOption> Options { get; set; } = [];
}
