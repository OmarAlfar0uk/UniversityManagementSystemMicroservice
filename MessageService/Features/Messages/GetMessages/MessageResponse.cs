namespace MessageService.Features.Messages.GetMessages;

public record MessageResponse(Guid Id, Guid SenderId, string Content, DateTime SentAt, bool IsRead);
