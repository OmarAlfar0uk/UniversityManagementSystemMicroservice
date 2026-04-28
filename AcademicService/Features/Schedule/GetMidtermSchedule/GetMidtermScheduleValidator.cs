using FluentValidation;

namespace AcademicService.Features.Schedule.GetMidtermSchedule;

public class GetMidtermScheduleValidator : AbstractValidator<GetMidtermScheduleQuery>
{
    public GetMidtermScheduleValidator()
    {
        RuleFor(x => x.DepartmentId).NotEmpty();
    }
}
