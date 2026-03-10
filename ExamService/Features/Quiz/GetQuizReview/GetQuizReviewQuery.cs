using ExamService.Contracts;
using MediatR;

namespace ExamService.Features.Quiz.GetQuizReview;

public record GetQuizReviewQuery(Guid LectureId, Guid StudentId) : IRequest<QuizReviewResponse>;
