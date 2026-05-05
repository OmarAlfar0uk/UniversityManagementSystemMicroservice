using MessageService.Contracts;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace MessageService.Features.Messages.GetMessages;

public class GetMessagesHandler : IRequestHandler<GetMessagesQuery, PagedResponse<MessageResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetMessagesHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PagedResponse<MessageResponse>> Handle(
        GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _unitOfWork.Messages.GetAllAsync(m => m.ConversationId == request.ConversationId);

        var ordered = messages.OrderByDescending(m => m.SentAt).ToList();
        var totalCount = ordered.Count;
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var currentUserId = Guid.Parse(_httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value ?? Guid.Empty.ToString());

        var items = ordered
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(m => new MessageResponse(
                m.Id,
                m.SenderId,
                IsMine: m.SenderId == currentUserId,
                m.Content ?? string.Empty,
                m.FileUrl,
                m.FileType.ToString(),
                m.IsRead,
                m.SentAt))
            .ToList();

        return new PagedResponse<MessageResponse>(items, request.PageNumber, request.PageSize, totalCount, totalPages);
    }
}
