using FluentValidation;

namespace ExamService.Features.Quiz.UpdateQuestion;

public class UpdateQuestionValidator : AbstractValidator<UpdateQuestionCommand>
{
    public UpdateQuestionValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.Text).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Points).GreaterThan(0);
    }
}
