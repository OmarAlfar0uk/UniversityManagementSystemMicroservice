using NotificationService.Contracts;
using MediatR;

namespace NotificationService.Features.Notifications.GetNotifications;

public class GetNotificationsHandler : IRequestHandler<GetNotificationsQuery, PagedResponse<NotificationResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetNotificationsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResponse<NotificationResponse>> Handle(
        GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var all = await _unitOfWork.Notifications.GetAllAsync(n => n.RecipientId == request.UserId);
        var ordered = all.OrderByDescending(n => n.CreatedAt).ToList();
        var total = ordered.Count;

        var items = ordered
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(n => new NotificationResponse(n.Id, n.Title, n.Body, n.IsRead, n.CreatedAt));

        var totalPages = (int)Math.Ceiling(total / (double)request.PageSize);
        return new PagedResponse<NotificationResponse>(items, request.PageNumber, request.PageSize, total, totalPages);
    }
}
