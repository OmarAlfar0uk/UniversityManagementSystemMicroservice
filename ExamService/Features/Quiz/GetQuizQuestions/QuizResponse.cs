namespace ExamService.Features.Quiz.GetQuizQuestions;

public record OptionResponse(Guid Id, string Text);

public record QuestionResponse(
    Guid Id,
    string Text,
    string Type,
    decimal Points,
    List<OptionResponse> Options
);

public record QuizResponse(
    Guid QuizId,
    int TimeLimitInMinutes,
    List<QuestionResponse> Questions
);
