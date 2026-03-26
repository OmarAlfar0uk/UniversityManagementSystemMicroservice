namespace MessageService.Features.AI.SendFile;

public record SendAiFileResponse(
    string? UserMessage,
    string? UserFileUrl,
    string AiReply,
    DateTime SentAt
);
