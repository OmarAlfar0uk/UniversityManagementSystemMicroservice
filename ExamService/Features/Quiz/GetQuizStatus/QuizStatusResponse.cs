namespace ExamService.Features.Quiz.GetQuizStatus;

public record QuizStatusResponse(
    int RemainingAttempts,
    int MaxAttempts,
    decimal? LastScore,
    bool IsPassed,
    bool CanAttempt
);
