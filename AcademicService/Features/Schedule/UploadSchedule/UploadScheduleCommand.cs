using AcademicService.Features.Schedule.GetClassSchedule;
using MediatR;

namespace AcademicService.Features.Schedule.UploadSchedule;

public record UploadScheduleCommand(
    IFormFile Image,
    string Type,
    Guid DepartmentId,
    string? AcademicYear
) : IRequest<ScheduleResponse>;
