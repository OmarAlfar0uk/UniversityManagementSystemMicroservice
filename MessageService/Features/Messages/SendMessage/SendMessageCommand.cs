using MediatR;

namespace MessageService.Features.Messages.SendMessage;

public record SendMessageCommand(Guid SenderId, Guid ReceiverId, string Content) : IRequest<SendMessageResponse>;
