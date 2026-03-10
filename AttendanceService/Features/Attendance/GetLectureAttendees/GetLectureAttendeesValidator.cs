using FluentValidation;

namespace AttendanceService.Features.Attendance.GetLectureAttendees;

public class GetLectureAttendeesValidator : AbstractValidator<GetLectureAttendeesQuery>
{
    public GetLectureAttendeesValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
    }
}
