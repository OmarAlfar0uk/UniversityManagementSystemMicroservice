using FluentValidation;

namespace AcademicService.Features.Courses.DeleteCourse;

public class DeleteCourseValidator : AbstractValidator<DeleteCourseCommand>
{
    public DeleteCourseValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
    }
}
