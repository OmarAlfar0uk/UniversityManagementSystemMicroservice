using FluentValidation;

namespace AttendanceService.Features.Attendance.GetLecturesAttendance;

public class GetLecturesAttendanceValidator : AbstractValidator<GetLecturesAttendanceQuery>
{
    public GetLecturesAttendanceValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
    }
}
