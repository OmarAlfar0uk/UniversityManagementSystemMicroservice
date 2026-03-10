using ExamService.Contracts;
using MediatR;

namespace ExamService.Features.Quiz.UpdateQuizSettings;

public class UpdateQuizSettingsHandler : IRequestHandler<UpdateQuizSettingsCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateQuizSettingsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateQuizSettingsCommand request, CancellationToken cancellationToken)
    {
        var quiz = await _unitOfWork.Quizzes.GetByIdAsync(request.QuizId);
        if (quiz is null)
            throw new KeyNotFoundException($"Quiz {request.QuizId} not found.");

        quiz.TimeLimitInMinutes = request.TimeLimitInMinutes;
        quiz.MaxAttempts = request.MaxAttempts;
        quiz.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Quizzes.Update(quiz);
        await _unitOfWork.SaveChangesAsync();
    }
}
