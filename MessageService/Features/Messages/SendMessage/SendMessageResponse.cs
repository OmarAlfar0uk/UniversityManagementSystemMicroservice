namespace MessageService.Features.Messages.SendMessage;

public record SendMessageResponse(Guid MessageId, Guid ConversationId, DateTime SentAt);
