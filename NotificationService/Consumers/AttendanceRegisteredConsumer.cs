using MassTransit;
using MediatR;
using NotificationService.Features.Notifications.SendNotification;
using Shered.Events;

namespace NotificationService.Consumers;

public class AttendanceRegisteredConsumer(IMediator mediator) : IConsumer<IAttendanceRegistered>
{
    public async Task Consume(ConsumeContext<IAttendanceRegistered> context)
    {
        try
        {
            var msg = context.Message;

            await mediator.Send(new SendNotificationCommand(
                msg.StudentId,
                "Attendance Confirmed",
                $"Your attendance for lecture on {msg.Date:dd MMM yyyy} has been recorded."
            ), context.CancellationToken);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[AttendanceRegisteredConsumer error] {ex.Message}");
        }
    }
}
