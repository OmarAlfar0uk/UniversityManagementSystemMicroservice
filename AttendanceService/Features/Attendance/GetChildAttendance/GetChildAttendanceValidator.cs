using FluentValidation;

namespace AttendanceService.Features.Attendance.GetChildAttendance;

public class GetChildAttendanceValidator : AbstractValidator<GetChildAttendanceQuery>
{
    public GetChildAttendanceValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();
    }
}
