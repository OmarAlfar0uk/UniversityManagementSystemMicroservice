using FluentValidation;

namespace ExamService.Features.Quiz.AddQuestion;

public class AddQuestionValidator : AbstractValidator<AddQuestionCommand>
{
    public AddQuestionValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
        RuleFor(x => x.Text).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Type).NotEmpty().Must(t => new[] { "MCQ", "TrueFalse", "Essay" }.Contains(t))
            .WithMessage("Type must be MCQ, TrueFalse, or Essay");
        RuleFor(x => x.Points).GreaterThan(0);
    }
}
