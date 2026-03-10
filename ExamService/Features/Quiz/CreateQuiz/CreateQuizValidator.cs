using FluentValidation;

namespace ExamService.Features.Quiz.CreateQuiz;

public class CreateQuizValidator : AbstractValidator<CreateQuizCommand>
{
    public CreateQuizValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.TimeLimitInMinutes).GreaterThan(0);
        RuleFor(x => x.MaxAttempts).GreaterThan(0);
    }
}
