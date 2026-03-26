using FluentValidation;

namespace ProgressService.Features.Progress.MarkVideoWatched;

public class MarkVideoWatchedValidator : AbstractValidator<MarkVideoWatchedCommand>
{
    public MarkVideoWatchedValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
    }
}
