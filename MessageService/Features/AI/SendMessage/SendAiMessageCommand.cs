using MediatR;

namespace MessageService.Features.AI.SendMessage;

public record SendAiMessageCommand(Guid StudentId, string Message) : IRequest<SendAiMessageResponse>;
