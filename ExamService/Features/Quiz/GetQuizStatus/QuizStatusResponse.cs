namespace ExamService.Features.Quiz.GetQuizStatus;

public record QuizStatusResponse(bool IsCompleted, Guid? AttemptId);
