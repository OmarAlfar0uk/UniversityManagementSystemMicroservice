using MessageService.Contracts;
using MediatR;

namespace MessageService.Features.Messages.GetMessages;

public class GetMessagesHandler : IRequestHandler<GetMessagesQuery, PagedResponse<MessageResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMessagesHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResponse<MessageResponse>> Handle(
        GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _unitOfWork.Messages.GetAllAsync(m => m.ConversationId == request.ConversationId);

        var ordered = messages.OrderByDescending(m => m.SentAt).ToList();
        var totalCount = ordered.Count;
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var items = ordered
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(m => new MessageResponse(m.Id, m.SenderId, m.Content ?? string.Empty, m.SentAt, m.IsRead))
            .ToList();

        return new PagedResponse<MessageResponse>(items, request.PageNumber, request.PageSize, totalCount, totalPages);
    }
}
