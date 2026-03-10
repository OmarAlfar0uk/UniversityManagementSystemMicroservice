using MessageService.Contracts;
using MediatR;

namespace MessageService.Features.Messages.GetConversations;

public class GetConversationsHandler : IRequestHandler<GetConversationsQuery, PagedResponse<ConversationResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetConversationsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResponse<ConversationResponse>> Handle(
        GetConversationsQuery request, CancellationToken cancellationToken)
    {
        var conversations = await _unitOfWork.Conversations.GetAllAsync(
            c => c.ParticipantAId == request.UserId || c.ParticipantBId == request.UserId);

        var ordered = conversations.OrderByDescending(c => c.LastMessageAt).ToList();
        var total = ordered.Count;

        var items = ordered
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c =>
            {
                var otherUserId = c.ParticipantAId == request.UserId ? c.ParticipantBId : c.ParticipantAId;
                return new ConversationResponse(c.Id, otherUserId, c.LastMessageAt);
            });

        return new PagedResponse<ConversationResponse>(items, total, request.PageNumber, request.PageSize);
    }
}
