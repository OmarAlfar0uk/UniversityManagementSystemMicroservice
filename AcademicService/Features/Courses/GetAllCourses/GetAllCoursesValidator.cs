using FluentValidation;

namespace AcademicService.Features.Courses.GetAllCourses;

public class GetAllCoursesValidator : AbstractValidator<GetAllCoursesQuery>
{
    public GetAllCoursesValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
