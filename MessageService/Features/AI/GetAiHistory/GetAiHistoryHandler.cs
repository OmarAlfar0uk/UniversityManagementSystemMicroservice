using MediatR;
using MessageService.Contracts;

namespace MessageService.Features.AI.GetAiHistory;

public class GetAiHistoryHandler(IUnitOfWork uow)
    : IRequestHandler<GetAiHistoryQuery, PagedResponse<AiMessageResponse>>
{
    public async Task<PagedResponse<AiMessageResponse>> Handle(
        GetAiHistoryQuery request, CancellationToken ct)
    {
        var all = (await uow.AiMessages.GetAllAsync(m => m.StudentId == request.StudentId))
            .OrderBy(m => m.SentAt)
            .ToList();

        var totalCount = all.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var data = all
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(m => new AiMessageResponse(
                m.Id,
                m.Role.ToString(),
                m.Content,
                m.FileUrl,
                m.SentAt))
            .ToList();

        return new PagedResponse<AiMessageResponse>(data, request.Page, request.PageSize, totalCount, totalPages);
    }
}
