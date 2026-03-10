using NotificationService.Contracts;
using NotificationService.Data.Models;
using MediatR;

namespace NotificationService.Features.Notifications.SendNotification;

public class SendNotificationHandler : IRequestHandler<SendNotificationCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public SendNotificationHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            RecipientId = request.UserId,
            Title = request.Title,
            Body = request.Body,
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();
    }
}
