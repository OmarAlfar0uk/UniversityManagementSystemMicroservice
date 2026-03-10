using FluentValidation;

namespace GradeService.Features.Grades.GetMidtermGrade;

public class GetMidtermGradeValidator : AbstractValidator<GetMidtermGradeQuery>
{
    public GetMidtermGradeValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
    }
}
