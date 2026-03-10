using AcademicService.Contracts;
using AcademicService.Data.Enums;
using AcademicService.Data.Models;
using AcademicService.Features.Schedule.GetClassSchedule;
using MediatR;

namespace AcademicService.Features.Schedule.GetMidtermSchedule;

public class GetMidtermScheduleHandler : IRequestHandler<GetMidtermScheduleQuery, IEnumerable<ScheduleResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMidtermScheduleHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ScheduleResponse>> Handle(GetMidtermScheduleQuery request, CancellationToken cancellationToken)
    {
        var schedules = await _unitOfWork.Schedules.GetAllAsync(s => s.Type == ScheduleType.MidtermExamSchedule);

        return schedules.Select(s => new ScheduleResponse(s.Id, s.ImageUrl ?? string.Empty, s.Type.ToString(), s.AcademicYear));
    }
}
