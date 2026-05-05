using MessageService.Contracts;
using MessageService.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace MessageService.Features.Messages.GetConversations;

public class GetConversationsHandler : IRequestHandler<GetConversationsQuery, PagedResponse<ConversationResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthServiceClient _authServiceClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetConversationsHandler(
        IUnitOfWork unitOfWork,
        IAuthServiceClient authServiceClient,
        IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _authServiceClient = authServiceClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PagedResponse<ConversationResponse>> Handle(
        GetConversationsQuery request, CancellationToken cancellationToken)
    {
        var conversations = await _unitOfWork.Conversations.GetAllAsync(
            c => c.StudentId == request.UserId || c.DoctorId == request.UserId);

        var ordered = conversations.OrderByDescending(c => c.UpdatedAt).ToList();
        var total = ordered.Count;
        var totalPages = (int)Math.Ceiling(total / (double)request.PageSize);

        var items = new List<ConversationResponse>();

        foreach (var c in ordered.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize))
        {
            var otherUserId = c.StudentId == request.UserId ? c.DoctorId : c.StudentId;

            string? otherUserName = null;
            var userInfo = await _authServiceClient.GetUserInfoAsync(otherUserId);
            if (userInfo is not null)
            {
                otherUserName = userInfo.FullName;
            }

            items.Add(new ConversationResponse(c.Id, otherUserId, otherUserName, c.UpdatedAt));
        }

        return new PagedResponse<ConversationResponse>(items, request.PageNumber, request.PageSize, total, totalPages);
    }
}
