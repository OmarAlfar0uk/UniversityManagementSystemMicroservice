using AcademicService.Features.Schedule.GetClassSchedule;
using MediatR;

namespace AcademicService.Features.Schedule.GetMidtermSchedule;

public record GetMidtermScheduleQuery(Guid DepartmentId)
    : IRequest<IEnumerable<ScheduleResponse>>;
