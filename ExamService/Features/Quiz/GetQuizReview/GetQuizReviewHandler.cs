using ExamService.Contracts;
using MediatR;

namespace ExamService.Features.Quiz.GetQuizReview;

public class GetQuizReviewHandler : IRequestHandler<GetQuizReviewQuery, QuizReviewResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetQuizReviewHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<QuizReviewResponse> Handle(GetQuizReviewQuery request, CancellationToken cancellationToken)
    {
        var quiz = await _unitOfWork.Quizzes.FindAsync(q => q.LectureId == request.LectureId);
        if (quiz is null)
            throw new KeyNotFoundException($"No quiz found for lecture {request.LectureId}.");

        var attempt = await _unitOfWork.QuizAttempts.FindAsync(
            a => a.QuizId == quiz.Id && a.StudentId == request.StudentId);
        if (attempt is null)
            throw new KeyNotFoundException("Quiz not yet attempted.");

        var answers = await _unitOfWork.QuizAnswers.GetAllAsync(a => a.QuizAttemptId == attempt.Id);

        var reviewAnswers = answers.Select(a => new ReviewAnswerResponse(
            a.QuizQuestionId,
            a.SelectedOptionId,
            a.AnswerText,
            a.IsCorrect ?? false,
            a.EarnedPoints ?? 0m
        )).ToList();

        return new QuizReviewResponse(
            attempt.Score ?? 0m,
            attempt.IsPassed == true ? "Passed" : "Failed",
            attempt.TimeTakenInMinutes ?? 0,
            reviewAnswers
        );
    }
}
