namespace MessageService.Features.Conversations.SendFile;

public record SendFileResponse(
    Guid MessageId,
    Guid ConversationId,
    string? FileUrl,
    string FileType,
    string? Content,
    DateTime SentAt
);
