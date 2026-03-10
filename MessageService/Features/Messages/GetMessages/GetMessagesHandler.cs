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
        var total = ordered.Count;

        var items = ordered
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(m => new MessageResponse(m.Id, m.SenderId, m.Content, m.SentAt, m.IsRead));

        return new PagedResponse<MessageResponse>(items, total, request.PageNumber, request.PageSize);
    }
}
