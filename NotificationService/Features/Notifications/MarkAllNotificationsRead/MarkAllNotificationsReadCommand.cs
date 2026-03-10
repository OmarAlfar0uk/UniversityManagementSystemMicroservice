using MediatR;

namespace NotificationService.Features.Notifications.MarkAllNotificationsRead;

public record MarkAllNotificationsReadCommand(Guid UserId) : IRequest;
