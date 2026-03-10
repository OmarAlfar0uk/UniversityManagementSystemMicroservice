using NotificationService.Contracts;
using MediatR;

namespace NotificationService.Features.Notifications.MarkAllNotificationsRead;

public class MarkAllNotificationsReadHandler : IRequestHandler<MarkAllNotificationsReadCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public MarkAllNotificationsReadHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(MarkAllNotificationsReadCommand request, CancellationToken cancellationToken)
    {
        var unread = await _unitOfWork.Notifications.GetAllAsync(n => n.RecipientId == request.UserId && !n.IsRead);

        foreach (var n in unread)
        {
            n.IsRead = true;
            n.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Notifications.Update(n);
        }

        await _unitOfWork.SaveChangesAsync();
    }
}
