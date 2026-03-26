using Microsoft.AspNetCore.SignalR;
using NotificationService.Contracts;
using NotificationService.Data.Models;
using NotificationService.Hubs;
using MediatR;

namespace NotificationService.Features.Notifications.SendNotification;

public class SendNotificationHandler(
    IUnitOfWork unitOfWork,
    IHubContext<NotificationHub> hubContext)
    : IRequestHandler<SendNotificationCommand>
{
    public async Task Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = new Notification
        {
            Id          = Guid.NewGuid(),
            RecipientId = request.UserId,
            Title       = request.Title,
            Body        = request.Body,
            IsRead      = false,
            CreatedAt   = DateTime.UtcNow,
            UpdatedAt   = DateTime.UtcNow
        };

        await unitOfWork.Notifications.AddAsync(notification);
        await unitOfWork.SaveChangesAsync();

        // ✅ Push real-time notification via SignalR
        try
        {
            await hubContext.Clients
                .Group(notification.RecipientId.ToString())
                .SendAsync("ReceiveNotification", new
                {
                    notification.Id,
                    notification.Title,
                    Message  = notification.Body,
                    notification.IsRead,
                    notification.CreatedAt
                }, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[SignalR push error] {ex.Message}");
        }
    }
}
