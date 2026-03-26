using MediatR;
using MessageService.Contracts;
using MessageService.Data.Enums;

namespace MessageService.Features.AI.DeleteHistory;

public class DeleteAiHistoryHandler(
    IUnitOfWork uow,
    IImageHelper imageHelper,
    IFileHelper fileHelper)
    : IRequestHandler<DeleteAiHistoryCommand>
{
    public async Task Handle(DeleteAiHistoryCommand request, CancellationToken ct)
    {
        var messages = (await uow.AiMessages.GetAllAsync(m => m.StudentId == request.StudentId))
            .ToList();

        // Delete all associated files from disk
        foreach (var msg in messages)
        {
            if (string.IsNullOrEmpty(msg.FileUrl)) continue;

            if (msg.FileType == FileType.Image || msg.FileType == FileType.Camera)
                imageHelper.DeleteImage(msg.FileUrl);
            else if (msg.FileType == FileType.Pdf)
                fileHelper.DeleteFile(msg.FileUrl);
        }

        uow.AiMessages.RemoveRange(messages);
        await uow.SaveChangesAsync();
    }
}
