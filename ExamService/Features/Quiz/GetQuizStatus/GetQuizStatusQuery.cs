using ExamService.Contracts;
using MediatR;

namespace ExamService.Features.Quiz.GetQuizStatus;

public record GetQuizStatusQuery(Guid LectureId, Guid StudentId) : IRequest<QuizStatusResponse>;
