using FluentValidation;

namespace AcademicService.Features.Courses.GetCourseInstructor;

public class GetCourseInstructorValidator : AbstractValidator<GetCourseInstructorQuery>
{
    public GetCourseInstructorValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
    }
}
