using FluentValidation;

namespace AttendanceService.Features.Attendance.GetCoursesAttendance;

public class GetCoursesAttendanceValidator : AbstractValidator<GetCoursesAttendanceQuery>
{
    public GetCoursesAttendanceValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();
    }
}
