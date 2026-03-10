using ExamService.Contracts;
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
        var quiz = await _unitOfWork.Quizzes.FindAsync(q => q.LectureId == request.LectureId);
        if (quiz is null)
            throw new KeyNotFoundException($"No quiz found for lecture {request.LectureId}.");

        var attempt = await _unitOfWork.QuizAttempts.FindAsync(
            a => a.QuizId == quiz.Id && a.StudentId == request.StudentId);

        return new QuizStatusResponse(
            IsCompleted: attempt is not null,
            AttemptId: attempt?.Id
        );
    }
}
