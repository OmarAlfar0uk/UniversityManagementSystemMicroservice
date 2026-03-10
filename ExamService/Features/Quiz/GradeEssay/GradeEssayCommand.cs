using MediatR;

namespace ExamService.Features.Quiz.GradeEssay;

public record GradeEssayCommand(Guid QuizId, Guid AttemptId, Guid QuestionId, decimal EarnedPoints) : IRequest;
