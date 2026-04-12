using ExamService.Contracts;
using MassTransit;
using MediatR;

namespace ExamService.Features.Quiz.GradeEssay;

public class GradeEssayHandler : IRequestHandler<GradeEssayCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;

    public GradeEssayHandler(IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint)
    {
        _unitOfWork = unitOfWork;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Handle(GradeEssayCommand request, CancellationToken cancellationToken)
    {
        var answer = await _unitOfWork.QuizAnswers.FindAsync(
            a => a.QuizAttemptId == request.AttemptId && a.QuizQuestionId == request.QuestionId);
        if (answer is null)
            throw new KeyNotFoundException("Quiz answer not found.");

        var question = await _unitOfWork.QuizQuestions.GetByIdAsync(request.QuestionId);
        if (question is null)
            throw new KeyNotFoundException("Question not found.");

        answer.EarnedPoints = request.EarnedPoints;
        answer.IsCorrect = request.EarnedPoints >= question.Points;
        answer.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.QuizAnswers.Update(answer);

        // Recalculate attempt score
        var attempt = await _unitOfWork.QuizAttempts.GetByIdAsync(request.AttemptId);
        if (attempt is not null)
        {
            var allAnswers = await _unitOfWork.QuizAnswers.GetAllAsync(a => a.QuizAttemptId == request.AttemptId);
            var questions = await _unitOfWork.QuizQuestions.GetAllAsync(q => q.QuizId == request.QuizId);
            var maxScore = questions.Sum(q => q.Points);
            var totalEarned = allAnswers.Sum(a => a.EarnedPoints ?? 0);
            attempt.Score = maxScore > 0 ? Math.Round(totalEarned / maxScore * 100, 2) : 0m;
            attempt.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.QuizAttempts.Update(attempt);
        }

        await _unitOfWork.SaveChangesAsync();

        if (attempt is not null)
        {
            await _publishEndpoint.Publish<Shered.Events.IQuizCompleted>(new
            {
                attempt.StudentId,
                attempt.QuizId,
                attempt.Score,
                IsPassed = attempt.Score >= 50, // Assuming 50 is passing
                CompletedAt = attempt.UpdatedAt
            });
        }
    }
}
