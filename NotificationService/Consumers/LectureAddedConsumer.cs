using MassTransit;
using MediatR;
using NotificationService.Features.Notifications.SendNotification;
using Shered.Events;

namespace NotificationService.Consumers;

public class LectureAddedConsumer(IMediator mediator) : IConsumer<ILectureAdded>
{
    public async Task Consume(ConsumeContext<ILectureAdded> context)
    {
        try
        {
            var msg = context.Message;

            foreach (var studentId in msg.StudentIds)
            {
                await mediator.Send(new SendNotificationCommand(
                    studentId,
                    "New Lecture Added",
                    $"A new lecture '{msg.LectureTitle}' has been added to {msg.CourseName}."
                ), context.CancellationToken);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[LectureAddedConsumer error] {ex.Message}");
        }
    }
}
