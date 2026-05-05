using ExamService.Contracts;
using ExamService.Data.Models;
using ExamService.Data.Enums;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Shered.Events;

namespace ExamService.Features.Quiz.SubmitQuiz;

public class SubmitQuizHandler : IRequestHandler<SubmitQuizCommand, QuizResultResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<SubmitQuizHandler> _logger;

    public SubmitQuizHandler(IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint, ILogger<SubmitQuizHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task<QuizResultResponse> Handle(SubmitQuizCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var quiz = await _unitOfWork.Quizzes.FindAsync(q => q.LectureId == request.LectureId);
            if (quiz is null)
                throw new KeyNotFoundException($"No quiz found for lecture {request.LectureId}.");

            var alreadyAttempted = await _unitOfWork.QuizAttempts.AnyAsync(
                a => a.QuizId == quiz.Id && a.StudentId == request.StudentId);
            if (alreadyAttempted)
                throw new InvalidOperationException("Quiz already submitted.");

            var startedAt = DateTime.UtcNow;
            var totalScore = 0m;
            var maxScore = 0m;

            var attempt = new QuizAttempt
            {
                Id = Guid.NewGuid(),
                QuizId = quiz.Id,
                StudentId = request.StudentId,
                StartedAt = startedAt,
                SubmittedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.QuizAttempts.AddAsync(attempt);
            await _unitOfWork.SaveChangesAsync(); // Save attempt first to get the ID

            foreach (var answer in request.Answers)
            {
                var question = await _unitOfWork.QuizQuestions.GetByIdAsync(answer.QuestionId);
                if (question is null) continue;

                maxScore += question.Points;

                bool? isCorrect = null;
                decimal? earnedPoints = null;

                if (question.Type != QuestionType.Essay)
                {
                    if (answer.SelectedOptionId.HasValue)
                    {
                        var option = await _unitOfWork.QuizQuestionOptions.GetByIdAsync(answer.SelectedOptionId.Value);
                        isCorrect = option?.IsCorrect ?? false;
                    }
                    else if (answer.AnswerText is not null)
                    {
                        isCorrect = string.Equals(
                            answer.AnswerText.Trim(),
                            question.CorrectAnswer?.Trim(),
                            StringComparison.OrdinalIgnoreCase);
                    }

                    earnedPoints = isCorrect == true ? question.Points : 0;
                    totalScore += earnedPoints ?? 0;
                }

                var quizAnswer = new QuizAnswer
                {
                    Id = Guid.NewGuid(),
                    QuizAttemptId = attempt.Id,
                    QuizQuestionId = answer.QuestionId,
                    SelectedOptionId = answer.SelectedOptionId,
                    AnswerText = answer.AnswerText,
                    IsCorrect = isCorrect,
                    EarnedPoints = earnedPoints,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.QuizAnswers.AddAsync(quizAnswer);
            }

            await _unitOfWork.SaveChangesAsync(); // Save all answers

            var scorePercentage = maxScore > 0 ? Math.Round(totalScore / maxScore * 100, 2) : 0m;
            attempt.Score = scorePercentage;
            attempt.IsPassed = scorePercentage >= 60;
            attempt.TimeTakenInMinutes = (int)(DateTime.UtcNow - startedAt).TotalMinutes;
            _unitOfWork.QuizAttempts.Update(attempt);

            await _unitOfWork.SaveChangesAsync(); // Save final attempt state

            // ✅ Publish event to RabbitMQ
            await _publishEndpoint.Publish<IQuizCompleted>(new
            {
                StudentId = attempt.StudentId,
                QuizId = attempt.QuizId,
                Score = attempt.Score,
                IsPassed = attempt.IsPassed ?? false,
                CompletedAt = attempt.SubmittedAt
            }, cancellationToken);

            return new QuizResultResponse(scorePercentage, attempt.IsPassed ?? false, attempt.TimeTakenInMinutes ?? 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting quiz for lecture {LectureId}, student {StudentId}", request.LectureId, request.StudentId);
            throw;
        }
    }
}
