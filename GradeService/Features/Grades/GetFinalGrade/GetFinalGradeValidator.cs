using FluentValidation;

namespace GradeService.Features.Grades.GetFinalGrade;

public class GetFinalGradeValidator : AbstractValidator<GetFinalGradeQuery>
{
    public GetFinalGradeValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
    }
}
