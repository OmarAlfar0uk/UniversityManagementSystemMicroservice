using MediatR;
using MessageService.Data.Enums;
using Microsoft.AspNetCore.Http;

namespace MessageService.Features.Conversations.SendFile;

public record SendFileCommand(
    Guid ConversationId,
    Guid SenderId,
    IFormFile File,
    string? Content,
    bool IsCamera = false
) : IRequest<SendFileResponse>;
