using ExamService.Contracts;
using ExamService.Data.Models;
using ExamService.Features.Quiz.GetQuizQuestions;
using MediatR;

namespace ExamService.Features.Quiz.CreateQuiz;

public class CreateQuizHandler : IRequestHandler<CreateQuizCommand, QuizResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateQuizHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<QuizResponse> Handle(CreateQuizCommand request, CancellationToken cancellationToken)
    {
        var quiz = new Data.Models.Quiz
        {
            Id = Guid.NewGuid(),
            LectureId = request.LectureId,
            CourseId = request.CourseId,
            TimeLimitInMinutes = request.TimeLimitInMinutes,
            MaxAttempts = request.MaxAttempts,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Quizzes.AddAsync(quiz);
        await _unitOfWork.SaveChangesAsync();

        return new QuizResponse(quiz.Id, quiz.TimeLimitInMinutes, new List<QuestionResponse>());
    }
}
