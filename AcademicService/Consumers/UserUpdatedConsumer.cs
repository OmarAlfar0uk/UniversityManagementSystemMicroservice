using AcademicService.Contracts;
using MassTransit;
using Shered.Events;

namespace AcademicService.Consumers;

public class UserUpdatedConsumer : IConsumer<UserUpdatedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserUpdatedConsumer> _logger;

    public UserUpdatedConsumer(IUnitOfWork unitOfWork, ILogger<UserUpdatedConsumer> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserUpdatedEvent> context)
    {
        var message = context.Message;
        var firstName = message.FirstName ?? string.Empty;
        var fullName = string.IsNullOrWhiteSpace(message.FullName)
            ? $"{message.FirstName} {message.LastName}".Trim()
            : message.FullName;
        var email = message.Email ?? string.Empty;
        var changed = false;

        var doctorCourses = await _unitOfWork.Courses.GetAllAsync(c => c.DoctorId == message.UserId);
        foreach (var course in doctorCourses)
        {
            course.DoctorFirstName = firstName;
            course.DoctorFullName = fullName;
            course.DoctorEmail = email;
            course.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Courses.Update(course);
            changed = true;
        }

        if (!changed)
            return;

        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Updated denormalized Academic doctor fields for {UserId}", message.UserId);
    }
}
