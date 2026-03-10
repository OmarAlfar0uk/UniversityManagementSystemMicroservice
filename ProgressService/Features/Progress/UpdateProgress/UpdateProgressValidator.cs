using FluentValidation;

namespace ProgressService.Features.Progress.UpdateProgress;

public class UpdateProgressValidator : AbstractValidator<UpdateProgressCommand>
{
    public UpdateProgressValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.TotalLecturesInCourse).GreaterThan(0);
    }
}
