using FluentValidation;

namespace AuthService.Features.Parent.GetChildCourses
{
    public class GetChildCoursesValidator : AbstractValidator<GetChildCoursesQuery>
    {
        public GetChildCoursesValidator()
        {
            RuleFor(x => x.ParentId)
                .NotEmpty().WithMessage("ParentId is required.");
        }
    }
}
