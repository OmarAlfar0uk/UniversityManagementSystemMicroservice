using ExamService.Contracts;
using MediatR;

namespace ExamService.Features.Quiz.GetQuizStatus;

public record GetQuizStatusQuery(Guid QuizOrLectureId, Guid StudentId, bool ByQuizId = false) : IRequest<QuizStatusResponse>;
