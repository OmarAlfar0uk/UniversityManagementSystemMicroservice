using MassTransit;
using MediatR;
using NotificationService.Contracts;
using NotificationService.Features.Notifications.SendNotification;

namespace NotificationService.Consumers
{
    public class AuthCreatedConsumer : IConsumer<IAuthCreated>
    {
        private readonly IMediator _mediator;
        private readonly INotificationAuditLogger _auditLogger;

        public AuthCreatedConsumer(IMediator mediator, INotificationAuditLogger auditLogger)
        {
            _mediator = mediator;
            _auditLogger = auditLogger;
        }

        public async Task Consume(ConsumeContext<IAuthCreated> context)
        {
            var eventData = context.Message;

            await _auditLogger.LogAsync(
                action: "ConsumeAuthCreated",
                targetId: eventData.UserId.ToString(),
                description: $"Received AuthCreated event for {eventData.Role}: {eventData.Email}"
            );

            // Automatically send a "Welcome" notification to the new user
            var command = new SendNotificationCommand(
                eventData.UserId,
                "Welcome to the University System!",
                $"Hello {eventData.UserName}, your account has been created successfully as a {eventData.Role}."
            );

            await _mediator.Send(command, context.CancellationToken);
        }
    }
}
