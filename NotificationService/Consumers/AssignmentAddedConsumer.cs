using MassTransit;
using MediatR;
using NotificationService.Features.Notifications.SendNotification;
using Shered.Events;

namespace NotificationService.Consumers;

public class AssignmentAddedConsumer(IMediator mediator) : IConsumer<IAssignmentAdded>
{
    public async Task Consume(ConsumeContext<IAssignmentAdded> context)
    {
        try
        {
            var msg = context.Message;
            var dueText = msg.DueDate.HasValue
                ? $" Due: {msg.DueDate.Value:dd MMM yyyy}."
                : string.Empty;

            foreach (var studentId in msg.StudentIds)
            {
                await mediator.Send(new SendNotificationCommand(
                    studentId,
                    "New Assignment",
                    $"A new assignment '{msg.Title}' has been added to {msg.CourseName}.{dueText}"
                ), context.CancellationToken);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[AssignmentAddedConsumer error] {ex.Message}");
        }
    }
}
