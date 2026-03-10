using NotificationService.Contracts;
using MediatR;

namespace NotificationService.Features.Notifications.MarkNotificationRead;

public class MarkNotificationReadHandler : IRequestHandler<MarkNotificationReadCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public MarkNotificationReadHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(request.NotificationId);
        if (notification is null)
            throw new KeyNotFoundException($"Notification {request.NotificationId} not found.");

        if (notification.RecipientId != request.UserId)
            throw new UnauthorizedAccessException("You do not own this notification.");

        notification.IsRead = true;
        notification.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Notifications.Update(notification);
        await _unitOfWork.SaveChangesAsync();
    }
}
