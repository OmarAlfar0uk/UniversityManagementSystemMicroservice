namespace ExamService.Features.Quiz.GetQuizResult;

public record QuizResultDetailResponse(decimal Score, bool IsPassed, int TimeTakenInMinutes, string Status);
