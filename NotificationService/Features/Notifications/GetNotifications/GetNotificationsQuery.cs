using NotificationService.Contracts;
using MediatR;

namespace NotificationService.Features.Notifications.GetNotifications;

public record GetNotificationsQuery(Guid UserId, int PageNumber = 1, int PageSize = 15) : IRequest<PagedResponse<NotificationResponse>>;
