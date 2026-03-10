using MessageService.Contracts;
using MediatR;

namespace MessageService.Features.Messages.GetConversations;

public record GetConversationsQuery(Guid UserId, int PageNumber = 1, int PageSize = 20) : IRequest<PagedResponse<ConversationResponse>>;
