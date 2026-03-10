using ExamService.Contracts;
using ExamService.Features.Quiz.GetQuizQuestions;
using MediatR;

namespace ExamService.Features.Quiz.UpdateQuestion;

public class UpdateQuestionHandler : IRequestHandler<UpdateQuestionCommand, QuestionResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateQuestionHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<QuestionResponse> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var question = await _unitOfWork.QuizQuestions.GetByIdAsync(request.QuestionId);
        if (question is null)
            throw new KeyNotFoundException($"Question {request.QuestionId} not found.");

        question.Text = request.Text;
        question.Points = request.Points;
        question.CorrectAnswer = request.CorrectAnswer;
        question.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.QuizQuestions.Update(question);
        await _unitOfWork.SaveChangesAsync();

        var options = await _unitOfWork.QuizQuestionOptions.GetAllAsync(o => o.QuizQuestionId == question.Id);
        return new QuestionResponse(
            question.Id,
            question.Text,
            question.Type.ToString(),
            question.Points,
            options.Select(o => new OptionResponse(o.Id, o.Text)).ToList()
        );
    }
}
