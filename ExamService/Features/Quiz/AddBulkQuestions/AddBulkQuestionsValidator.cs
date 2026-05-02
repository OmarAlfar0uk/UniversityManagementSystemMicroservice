using FluentValidation;

namespace ExamService.Features.Quiz.AddBulkQuestions;

public class AddBulkQuestionsValidator : AbstractValidator<AddBulkQuestionsCommand>
{
    public AddBulkQuestionsValidator()
    {
        RuleFor(x => x.QuizId).NotEmpty();
        RuleFor(x => x.Questions)
            .NotNull()
            .NotEmpty();

        RuleForEach(x => x.Questions).ChildRules(q =>
        {
            q.RuleFor(v => v.Text).NotEmpty().MaximumLength(1000);
            q.RuleFor(v => v.Type).NotEmpty().Must(t => new[] { "MCQ", "TrueFalse", "Essay" }.Contains(t))
                .WithMessage("Type must be MCQ, TrueFalse, or Essay");
            q.RuleFor(v => v.Points).GreaterThan(0);
        });
    }
}
