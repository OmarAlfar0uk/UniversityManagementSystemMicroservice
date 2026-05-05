using ExamService.Contracts;
using ExamService.Middlewares;
using MediatR;

namespace ExamService.Features.Quiz.GetQuizStatus;

public class GetQuizStatusHandler : IRequestHandler<GetQuizStatusQuery, QuizStatusResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetQuizStatusHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<QuizStatusResponse> Handle(GetQuizStatusQuery request, CancellationToken cancellationToken)
    {
        var quiz = request.ByQuizId
            ? await _unitOfWork.Quizzes.GetByIdAsync(request.QuizOrLectureId)
            : await _unitOfWork.Quizzes.FindAsync(q => q.LectureId == request.QuizOrLectureId);

        if (quiz is null)
            throw new KeyNotFoundException(
                request.ByQuizId
                    ? $"Quiz {request.QuizOrLectureId} not found."
                    : $"No quiz found for lecture {request.QuizOrLectureId}.");

        var attempts = (await _unitOfWork.QuizAttempts.GetAllAsync(
                a => a.QuizId == quiz.Id && a.StudentId == request.StudentId && a.SubmittedAt != null))
            .OrderByDescending(a => a.SubmittedAt)
            .ToList();

        var usedAttempts = attempts.Count;
        var remainingAttempts = Math.Max(quiz.MaxAttempts - usedAttempts, 0);
        if (request.ByQuizId && remainingAttempts == 0)
            throw new ForbiddenException("No remaining attempts.");

        var lastAttempt = attempts.FirstOrDefault();

        return new QuizStatusResponse(
            remainingAttempts,
            quiz.MaxAttempts,
            lastAttempt?.Score,
            lastAttempt?.IsPassed ?? false,
            remainingAttempts > 0 && quiz.IsActive
        );
    }
}
