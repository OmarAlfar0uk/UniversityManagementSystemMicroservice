using FluentValidation;

namespace GradeService.Features.Grades.SetMidtermGrade;

public class SetMidtermGradeValidator : AbstractValidator<SetMidtermGradeCommand>
{
    public SetMidtermGradeValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.Score).InclusiveBetween(0, 100);
    }
}
