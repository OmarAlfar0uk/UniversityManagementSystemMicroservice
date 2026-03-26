using MediatR;
using NotificationService.Contracts;
using NotificationService.Features.Notifications;

namespace NotificationService.Features.Notifications.DeleteNotification;

public class DeleteNotificationHandler : IRequestHandler<DeleteNotificationCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteNotificationHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(request.NotificationId);
        
        if (notification == null || notification.RecipientId != request.UserId)
            throw new KeyNotFoundException($"Notification {request.NotificationId} not found.");

        _unitOfWork.Notifications.Remove(notification);
        await _unitOfWork.SaveChangesAsync();
    }
}
