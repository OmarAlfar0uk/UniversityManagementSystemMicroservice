namespace NotificationService.Features.Notifications.GetNotifications;

public record NotificationResponse(Guid Id, string Title, string Body, bool IsRead, DateTime CreatedAt);
