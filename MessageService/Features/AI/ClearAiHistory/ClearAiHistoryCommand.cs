using MediatR;
using MessageService.Contracts;
using MessageService.Data.Models;

namespace MessageService.Features.AI.ClearAiHistory;

public record ClearAiHistoryCommand(Guid StudentId) : IRequest;
