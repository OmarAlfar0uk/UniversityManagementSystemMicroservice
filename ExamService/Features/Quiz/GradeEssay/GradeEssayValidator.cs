using FluentValidation;

namespace ExamService.Features.Quiz.GradeEssay;

public class GradeEssayValidator : AbstractValidator<GradeEssayCommand>
{
    public GradeEssayValidator()
    {
        RuleFor(x => x.QuizId).NotEmpty();
        RuleFor(x => x.AttemptId).NotEmpty();
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.EarnedPoints).GreaterThanOrEqualTo(0);
    }
}
