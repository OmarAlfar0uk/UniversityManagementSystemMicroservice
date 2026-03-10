using FluentValidation;

namespace AttendanceService.Features.Attendance.GetAttendanceStatus;

public class GetAttendanceStatusValidator : AbstractValidator<GetAttendanceStatusQuery>
{
    public GetAttendanceStatusValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
    }
}
