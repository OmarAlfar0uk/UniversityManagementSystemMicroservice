namespace ExamService.Features.Quiz.GetQuizReview;

public record ReviewAnswerResponse(
    Guid QuestionId,
    Guid? SelectedOptionId,
    string? AnswerText,
    bool? IsCorrect,
    decimal? EarnedPoints
);

public record QuizReviewResponse(
    decimal TotalScore,
    string Status,
    int TimeTaken,
    List<ReviewAnswerResponse> Questions
);
