using AcademicService.Contracts;
using AcademicService.Data.Enums;
using AcademicService.Features.Schedule.GetClassSchedule;
using MediatR;

namespace AcademicService.Features.Schedule.GetMidtermSchedule;

public class GetMidtermScheduleHandler
    : IRequestHandler<GetMidtermScheduleQuery, IEnumerable<ScheduleResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageHelper _imageHelper;

    public GetMidtermScheduleHandler(IUnitOfWork unitOfWork, IImageHelper imageHelper)
    {
        _unitOfWork = unitOfWork;
        _imageHelper = imageHelper;
    }

    public async Task<IEnumerable<ScheduleResponse>> Handle(
        GetMidtermScheduleQuery request,
        CancellationToken cancellationToken)
    {
        var schedules = await _unitOfWork.Schedules.GetAllAsync(s =>
            s.Type == ScheduleType.MidtermExamSchedule &&
            s.DepartmentId == request.DepartmentId);

        return schedules.Select(s => new ScheduleResponse(
            s.Id,
            _imageHelper.GetImageUrl(s.ImageUrl) ?? string.Empty,
            s.Type.ToString(),
            s.AcademicYear,
            s.DepartmentId
        ));
    }
}
