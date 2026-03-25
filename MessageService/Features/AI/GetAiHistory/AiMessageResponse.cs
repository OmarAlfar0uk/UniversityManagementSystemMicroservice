namespace MessageService.Features.AI.GetAiHistory;

public record AiMessageResponse(
    Guid Id,
    string Role,
    string Content,
    string? FileUrl,
    DateTime SentAt
);
