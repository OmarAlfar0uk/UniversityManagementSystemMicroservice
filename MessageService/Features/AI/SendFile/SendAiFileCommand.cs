using MediatR;
using Microsoft.AspNetCore.Http;

namespace MessageService.Features.AI.SendFile;

public record SendAiFileCommand(
    Guid StudentId,
    IFormFile File,
    string? Message
) : IRequest<SendAiFileResponse>;
