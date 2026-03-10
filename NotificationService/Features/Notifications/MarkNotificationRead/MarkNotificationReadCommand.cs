using MediatR;

namespace NotificationService.Features.Notifications.MarkNotificationRead;

public record MarkNotificationReadCommand(Guid NotificationId, Guid UserId) : IRequest;
