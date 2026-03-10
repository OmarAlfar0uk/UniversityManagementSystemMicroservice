using AcademicService.Contracts;
using AcademicService.Data.Models;
using AcademicService.Features.Schedule.GetClassSchedule;
using MediatR;

namespace AcademicService.Features.Schedule.GetMidtermSchedule;

public record GetMidtermScheduleQuery() : IRequest<IEnumerable<ScheduleResponse>>;
