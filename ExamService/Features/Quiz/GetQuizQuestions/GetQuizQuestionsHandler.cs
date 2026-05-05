using ExamService.Contracts;
using ExamService.Data.Models;
using ExamService.Middlewares;
using MediatR;

namespace ExamService.Features.Quiz.GetQuizQuestions;

public class GetQuizQuestionsHandler : IRequestHandler<GetQuizQuestionsQuery, QuizResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetQuizQuestionsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<QuizResponse> Handle(GetQuizQuestionsQuery request, CancellationToken cancellationToken)
    {
        var quiz = await _unitOfWork.Quizzes.FindAsync(q => q.LectureId == request.LectureId);
        if (quiz is null)
            throw new KeyNotFoundException($"No quiz found for lecture {request.LectureId}.");

        if (!quiz.IsActive)
            throw new ForbiddenException("Quiz is not active.");

        var submittedAttempts = await _unitOfWork.QuizAttempts.CountAsync(
            a => a.QuizId == quiz.Id && a.StudentId == request.StudentId && a.SubmittedAt != null);

        if (submittedAttempts >= quiz.MaxAttempts)
            throw new ForbiddenException("No remaining attempts.");

        var openAttempt = await _unitOfWork.QuizAttempts.FindAsync(
            a => a.QuizId == quiz.Id && a.StudentId == request.StudentId && a.SubmittedAt == null);

        if (openAttempt is null)
        {
            await _unitOfWork.QuizAttempts.AddAsync(new QuizAttempt
            {
                Id = Guid.NewGuid(),
                QuizId = quiz.Id,
                StudentId = request.StudentId,
                StartedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await _unitOfWork.SaveChangesAsync();
        }

        var questions = await _unitOfWork.QuizQuestions.GetAllAsync(q => q.QuizId == quiz.Id);

        var questionResponses = new List<QuestionResponse>();
        foreach (var q in questions.OrderBy(q => q.OrderIndex))
        {
            var options = await _unitOfWork.QuizQuestionOptions.GetAllAsync(o => o.QuizQuestionId == q.Id);
            questionResponses.Add(new QuestionResponse(
                q.Id,
                q.Text,
                q.Type.ToString(),
                q.Points,
                options.Select(o => new OptionResponse(o.Id, o.Text, o.IsCorrect)).ToList()
            ));
        }

        return new QuizResponse(quiz.Id, quiz.TimeLimitInMinutes, questionResponses);
    }
}
