using FluentValidation;

namespace GradeService.Features.Grades.GetGPA;

public class GetGPAValidator : AbstractValidator<GetGPAQuery>
{
    public GetGPAValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();
    }
}
