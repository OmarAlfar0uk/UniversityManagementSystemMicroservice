using MediatR;

namespace AcademicService.Features.Schedule.GetClassSchedule;

public record GetClassScheduleQuery(Guid DepartmentId)
    : IRequest<IEnumerable<ScheduleResponse>>;
