using AcademicService.Contracts;
using AcademicService.Data.Enums;
using MediatR;

namespace AcademicService.Features.Schedule.GetClassSchedule;

public class GetClassScheduleHandler
    : IRequestHandler<GetClassScheduleQuery, IEnumerable<ScheduleResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageHelper _imageHelper;

    public GetClassScheduleHandler(IUnitOfWork unitOfWork, IImageHelper imageHelper)
    {
        _unitOfWork = unitOfWork;
        _imageHelper = imageHelper;
    }

    public async Task<IEnumerable<ScheduleResponse>> Handle(
        GetClassScheduleQuery request,
        CancellationToken cancellationToken)
    {
        var schedules = await _unitOfWork.Schedules.GetAllAsync(s =>
            s.Type == ScheduleType.ClassSchedule &&
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
