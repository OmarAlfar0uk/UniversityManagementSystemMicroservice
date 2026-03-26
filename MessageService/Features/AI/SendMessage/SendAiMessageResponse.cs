namespace MessageService.Features.AI.SendMessage;

public record SendAiMessageResponse(
    string UserMessage,
    string AiReply,
    DateTime SentAt
);
