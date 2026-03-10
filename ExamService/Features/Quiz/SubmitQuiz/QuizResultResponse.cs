namespace ExamService.Features.Quiz.SubmitQuiz;

public record QuizResultResponse(decimal Score, bool IsPassed, int TimeTakenInMinutes);
