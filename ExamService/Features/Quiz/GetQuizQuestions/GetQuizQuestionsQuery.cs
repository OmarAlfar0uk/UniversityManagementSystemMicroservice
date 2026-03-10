using ExamService.Contracts;
using MediatR;

namespace ExamService.Features.Quiz.GetQuizQuestions;

public record GetQuizQuestionsQuery(Guid LectureId) : IRequest<QuizResponse>;
