using ExamService.Contracts;
using MediatR;

namespace ExamService.Features.Quiz.GetQuizResult;

public class GetQuizResultHandler : IRequestHandler<GetQuizResultQuery, QuizResultDetailResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetQuizResultHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<QuizResultDetailResponse> Handle(GetQuizResultQuery request, CancellationToken cancellationToken)
    {
        var quiz = await _unitOfWork.Quizzes.FindAsync(q => q.LectureId == request.LectureId);
        if (quiz is null)
            throw new KeyNotFoundException($"No quiz found for lecture {request.LectureId}.");

        var attempt = await _unitOfWork.QuizAttempts.FindAsync(
            a => a.QuizId == quiz.Id && a.StudentId == request.StudentId);
        if (attempt is null)
            throw new KeyNotFoundException("Quiz not yet attempted.");

        return new QuizResultDetailResponse(
            attempt.Score ?? 0m,
            attempt.IsPassed ?? false,
            attempt.TimeTakenInMinutes ?? 0,
            attempt.IsPassed == true ? "Passed" : "Failed"
        );
    }
}
