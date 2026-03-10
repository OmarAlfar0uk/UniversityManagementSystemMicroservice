using FluentValidation;

namespace GradeService.Features.Grades.SetFinalGrade;

public class SetFinalGradeValidator : AbstractValidator<SetFinalGradeCommand>
{
    public SetFinalGradeValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.Score).InclusiveBetween(0, 100);
    }
}
