using FluentValidation;

namespace AttendanceService.Features.Attendance.RegisterAttendance;

public class RegisterAttendanceValidator : AbstractValidator<RegisterAttendanceCommand>
{
    public RegisterAttendanceValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
    }
}
