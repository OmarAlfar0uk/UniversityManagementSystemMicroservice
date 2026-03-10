using FluentValidation;

namespace AcademicService.Features.LectureMaterials.MarkVideoWatched;

public class MarkVideoWatchedValidator : AbstractValidator<MarkVideoWatchedCommand>
{
    public MarkVideoWatchedValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
    }
}
