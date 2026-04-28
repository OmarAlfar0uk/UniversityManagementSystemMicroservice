using FluentValidation;

namespace AcademicService.Features.Schedule.GetClassSchedule;

public class GetClassScheduleValidator : AbstractValidator<GetClassScheduleQuery>
{
    public GetClassScheduleValidator()
    {
        RuleFor(x => x.DepartmentId).NotEmpty();
    }
}
