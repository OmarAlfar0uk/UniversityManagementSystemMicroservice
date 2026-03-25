using MediatR;
using MessageService.Data.Enums;

namespace MessageService.Features.AI.AskRashed;

public record AskRashedCommand(Guid StudentId, string Message, string? FileUrl, FileType FileType)
    : IRequest<AskRashedResponse>;
