using FluentValidation;

namespace AttendanceService.Features.Attendance.GetAttendancePercentage;

public class GetAttendancePercentageValidator : AbstractValidator<GetAttendancePercentageQuery>
{
    public GetAttendancePercentageValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
    }
}
