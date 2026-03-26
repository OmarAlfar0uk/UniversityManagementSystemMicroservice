using MassTransit;
using MediatR;
using NotificationService.Features.Notifications.SendNotification;
using Shered.Events;

namespace NotificationService.Consumers;

public class GradeAddedConsumer(IMediator mediator) : IConsumer<IGradeAdded>
{
    public async Task Consume(ConsumeContext<IGradeAdded> context)
    {
        try
        {
            var msg = context.Message;

            await mediator.Send(new SendNotificationCommand(
                msg.StudentId,
                $"{msg.GradeType} Grade Posted",
                $"Your {msg.GradeType} grade has been posted: {msg.Score:0.##}."
            ), context.CancellationToken);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[GradeAddedConsumer error] {ex.Message}");
        }
    }
}
