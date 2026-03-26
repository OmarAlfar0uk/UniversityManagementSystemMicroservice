using FluentValidation;

namespace ProgressService.Features.Progress.MarkPdfViewed;

public class MarkPdfViewedValidator : AbstractValidator<MarkPdfViewedCommand>
{
    public MarkPdfViewedValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
    }
}
