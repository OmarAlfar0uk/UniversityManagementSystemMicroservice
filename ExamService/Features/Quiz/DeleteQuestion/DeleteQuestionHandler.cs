using ExamService.Contracts;
using MediatR;

namespace ExamService.Features.Quiz.DeleteQuestion;

public class DeleteQuestionHandler : IRequestHandler<DeleteQuestionCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteQuestionHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
    {
        var question = await _unitOfWork.QuizQuestions.GetByIdAsync(request.QuestionId);
        if (question is null)
            throw new KeyNotFoundException($"Question {request.QuestionId} not found.");

        var options = await _unitOfWork.QuizQuestionOptions.GetAllAsync(o => o.QuizQuestionId == request.QuestionId);
        _unitOfWork.QuizQuestionOptions.RemoveRange(options);
        _unitOfWork.QuizQuestions.Remove(question);
        await _unitOfWork.SaveChangesAsync();
    }
}
