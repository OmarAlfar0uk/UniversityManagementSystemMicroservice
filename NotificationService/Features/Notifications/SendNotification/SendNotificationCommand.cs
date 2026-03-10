using MediatR;

namespace NotificationService.Features.Notifications.SendNotification;

public record SendNotificationCommand(
    Guid UserId,
    string Title,
    string Body
) : IRequest;
