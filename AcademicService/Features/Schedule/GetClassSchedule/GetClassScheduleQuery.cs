using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Schedule.GetClassSchedule;

public record GetClassScheduleQuery() : IRequest<IEnumerable<ScheduleResponse>>;
