using FluentValidation;

namespace AttendanceService.Features.Attendance.ManualAttendance;

public class ManualAttendanceValidator : AbstractValidator<ManualAttendanceCommand>
{
    public ManualAttendanceValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
    }
}
