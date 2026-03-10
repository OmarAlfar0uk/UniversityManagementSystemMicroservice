using AcademicService.Contracts;
using AcademicService.Data.Enums;
using AcademicService.Data.Models;
using MediatR;

namespace AcademicService.Features.Schedule.GetClassSchedule;

public class GetClassScheduleHandler : IRequestHandler<GetClassScheduleQuery, IEnumerable<ScheduleResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetClassScheduleHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ScheduleResponse>> Handle(GetClassScheduleQuery request, CancellationToken cancellationToken)
    {
        var schedules = await _unitOfWork.Schedules.GetAllAsync(s => s.Type == ScheduleType.ClassSchedule);

        return schedules.Select(s => new ScheduleResponse(s.Id, s.ImageUrl ?? string.Empty, s.Type.ToString(), s.AcademicYear));
    }
}
