using MediatR;
using MessageService.Contracts;

namespace MessageService.Features.AI.GetAiHistory;

public record GetAiHistoryQuery(Guid StudentId, int Page, int PageSize)
    : IRequest<PagedResponse<AiMessageResponse>>;
