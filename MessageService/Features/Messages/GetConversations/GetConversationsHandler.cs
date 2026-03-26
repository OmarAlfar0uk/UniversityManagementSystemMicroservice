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
            c => c.StudentId == request.UserId || c.DoctorId == request.UserId);

        var ordered = conversations.OrderByDescending(c => c.UpdatedAt).ToList();
        var total = ordered.Count;
        var totalPages = (int)Math.Ceiling(total / (double)request.PageSize);

        var items = ordered
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c =>
            {
                var otherUserId = c.StudentId == request.UserId ? c.DoctorId : c.StudentId;
                return new ConversationResponse(c.Id, otherUserId, c.UpdatedAt);
            })
            .ToList();

        return new PagedResponse<ConversationResponse>(items, request.PageNumber, request.PageSize, total, totalPages);
    }
}
