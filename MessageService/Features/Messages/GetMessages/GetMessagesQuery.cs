using MessageService.Contracts;
using MediatR;

namespace MessageService.Features.Messages.GetMessages;

public record GetMessagesQuery(Guid ConversationId, int PageNumber = 1, int PageSize = 30) : IRequest<PagedResponse<MessageResponse>>;
