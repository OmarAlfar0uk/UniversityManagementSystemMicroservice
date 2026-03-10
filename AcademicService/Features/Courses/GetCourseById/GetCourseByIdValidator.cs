using FluentValidation;

namespace AcademicService.Features.Courses.GetCourseById;

public class GetCourseByIdValidator : AbstractValidator<GetCourseByIdQuery>
{
    public GetCourseByIdValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
    }
}
