using ExamService.Contracts;
using ExamService.Features.Quiz.GetQuizQuestions;
using MediatR;

namespace ExamService.Features.Quiz.CreateQuiz;

public record CreateQuizCommand(
    Guid LectureId,
    Guid CourseId,
    int TimeLimitInMinutes,
    int MaxAttempts
) : IRequest<QuizResponse>;
