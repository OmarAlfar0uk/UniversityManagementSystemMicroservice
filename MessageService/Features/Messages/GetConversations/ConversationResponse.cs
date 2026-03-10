namespace MessageService.Features.Messages.GetConversations;

public record ConversationResponse(Guid ConversationId, Guid OtherUserId, DateTime? LastMessageAt);
