using MediatR;
using MessageService.Contracts;

namespace MessageService.Features.AI.ClearAiHistory;

public class ClearAiHistoryHandler(IUnitOfWork uow) : IRequestHandler<ClearAiHistoryCommand>
{
    public async Task Handle(ClearAiHistoryCommand request, CancellationToken ct)
    {
        var messages = await uow.AiMessages.GetAllAsync(m => m.StudentId == request.StudentId);
        uow.AiMessages.RemoveRange(messages);

        await uow.SaveChangesAsync();
    }
}
