using MassTransit;
using MediatR;
using NotificationService.Features.Notifications.SendNotification;
using Shered.Events;

namespace NotificationService.Consumers;

public class QuizCompletedConsumer(IMediator mediator) : IConsumer<IQuizCompleted>
{
    public async Task Consume(ConsumeContext<IQuizCompleted> context)
    {
        try
        {
            var msg = context.Message;

            await mediator.Send(new SendNotificationCommand(
                msg.StudentId,
                "Quiz Result",
                $"You scored {msg.Score:0.##}% on your quiz. Status: {(msg.IsPassed ? "Passed ✅" : "Failed ❌")}."
            ), context.CancellationToken);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[QuizCompletedConsumer error] {ex.Message}");
        }
    }
}
