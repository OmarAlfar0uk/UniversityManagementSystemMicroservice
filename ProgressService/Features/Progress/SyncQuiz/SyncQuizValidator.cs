using FluentValidation;

namespace ProgressService.Features.Progress.SyncQuiz;

public class SyncQuizValidator : AbstractValidator<SyncQuizCommand>
{
    public SyncQuizValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
    }
}
