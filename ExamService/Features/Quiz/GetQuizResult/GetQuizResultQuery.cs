using ExamService.Contracts;
using ExamService.Features.Quiz.SubmitQuiz;
using MediatR;

namespace ExamService.Features.Quiz.GetQuizResult;

public record GetQuizResultQuery(Guid LectureId, Guid StudentId) : IRequest<QuizResultDetailResponse>;
