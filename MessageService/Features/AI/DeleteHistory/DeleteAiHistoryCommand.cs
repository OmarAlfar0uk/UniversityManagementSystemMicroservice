using MediatR;

namespace MessageService.Features.AI.DeleteHistory;

public record DeleteAiHistoryCommand(Guid StudentId) : IRequest;
