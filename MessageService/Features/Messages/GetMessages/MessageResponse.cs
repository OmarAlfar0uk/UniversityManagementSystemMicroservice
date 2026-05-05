namespace MessageService.Features.Messages.GetMessages;

public record MessageResponse(
    Guid Id,
    Guid SenderId,
    bool IsMine,
    string Content,
    string? FileUrl,
    string? FileType,
    bool IsRead,
    DateTime SentAt
);
