using FluentValidation;

namespace ExamService.Features.Quiz.DeleteQuestion;

public class DeleteQuestionValidator : AbstractValidator<DeleteQuestionCommand>
{
    public DeleteQuestionValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
    }
}
