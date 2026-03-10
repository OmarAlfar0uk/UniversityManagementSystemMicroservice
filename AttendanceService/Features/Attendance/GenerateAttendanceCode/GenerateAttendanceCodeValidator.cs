using FluentValidation;

namespace AttendanceService.Features.Attendance.GenerateAttendanceCode;

public class GenerateAttendanceCodeValidator : AbstractValidator<GenerateAttendanceCodeCommand>
{
    public GenerateAttendanceCodeValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.ExpiresInMinutes).InclusiveBetween(1, 1440);
    }
}
