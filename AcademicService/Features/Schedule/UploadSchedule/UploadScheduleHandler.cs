using AcademicService.Contracts;
using AcademicService.Data.Enums;
using AcademicService.Features.Schedule.GetClassSchedule;
using MediatR;

namespace AcademicService.Features.Schedule.UploadSchedule;

public class UploadScheduleHandler : IRequestHandler<UploadScheduleCommand, ScheduleResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageHelper _imageHelper;

    public UploadScheduleHandler(IUnitOfWork unitOfWork, IImageHelper imageHelper)
    {
        _unitOfWork = unitOfWork;
        _imageHelper = imageHelper;
    }

    public async Task<ScheduleResponse> Handle(
        UploadScheduleCommand request,
        CancellationToken cancellationToken)
    {
        // Parse enum
        if (!Enum.TryParse<ScheduleType>(request.Type, out var scheduleType))
            throw new ArgumentException($"Invalid schedule type: {request.Type}");

        // Delete existing schedule for this department + type if exists
        var existing = await _unitOfWork.Schedules.FindAsync(s =>
            s.DepartmentId == request.DepartmentId && s.Type == scheduleType);

        if (existing is not null)
        {
            // Delete old image file
            _imageHelper.DeleteImage(existing.ImageUrl);
            _unitOfWork.Schedules.Remove(existing);
        }

        // Save new image
        var relativePath = await _imageHelper.SaveImageAsync(
            request.Image, $"Schedules/{request.DepartmentId}");

        // Create new schedule record
        var schedule = new Data.Models.Schedule
        {
            Id = Guid.NewGuid(),
            ImageUrl = relativePath,
            Type = scheduleType,
            DepartmentId = request.DepartmentId,
            AcademicYear = request.AcademicYear,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Schedules.AddAsync(schedule);
        await _unitOfWork.SaveChangesAsync();

        return new ScheduleResponse(
            schedule.Id,
            _imageHelper.GetImageUrl(schedule.ImageUrl) ?? string.Empty,
            schedule.Type.ToString(),
            schedule.AcademicYear,
            schedule.DepartmentId
        );
    }
}
