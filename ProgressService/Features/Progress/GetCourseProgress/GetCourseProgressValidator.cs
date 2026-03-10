using FluentValidation;

namespace ProgressService.Features.Progress.GetCourseProgress;

public class GetCourseProgressValidator : AbstractValidator<GetCourseProgressQuery>
{
    public GetCourseProgressValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
    }
}
