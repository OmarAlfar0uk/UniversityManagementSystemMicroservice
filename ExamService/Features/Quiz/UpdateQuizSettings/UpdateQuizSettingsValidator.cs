using FluentValidation;

namespace ExamService.Features.Quiz.UpdateQuizSettings;

public class UpdateQuizSettingsValidator : AbstractValidator<UpdateQuizSettingsCommand>
{
    public UpdateQuizSettingsValidator()
    {
        RuleFor(x => x.QuizId).NotEmpty();
        RuleFor(x => x.TimeLimitInMinutes).GreaterThan(0);
        RuleFor(x => x.MaxAttempts).GreaterThan(0);
    }
}
