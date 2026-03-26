using FluentValidation;

namespace ProgressService.Features.Progress.SyncAttendance;

public class SyncAttendanceValidator : AbstractValidator<SyncAttendanceCommand>
{
    public SyncAttendanceValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
    }
}
