using FluentValidation;

namespace ProgressService.Features.Progress.SyncAssignment;

public class SyncAssignmentValidator : AbstractValidator<SyncAssignmentCommand>
{
    public SyncAssignmentValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
    }
}
