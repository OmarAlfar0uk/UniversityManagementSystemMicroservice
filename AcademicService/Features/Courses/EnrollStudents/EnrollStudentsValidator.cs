using FluentValidation;

namespace AcademicService.Features.Courses.EnrollStudents;

public class EnrollStudentsValidator : AbstractValidator<EnrollStudentsCommand>
{
    public EnrollStudentsValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.StudentIds).NotEmpty().Must(ids => ids.Count > 0)
            .WithMessage("At least one StudentId is required.");
    }
}
