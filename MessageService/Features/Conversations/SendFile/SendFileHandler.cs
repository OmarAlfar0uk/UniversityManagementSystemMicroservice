using MediatR;
using MessageService.Contracts;
using MessageService.Data.Enums;
using MessageService.Data.Models;

namespace MessageService.Features.Conversations.SendFile;

public class SendFileHandler(
    IUnitOfWork uow,
    IImageHelper imageHelper,
    IFileHelper fileHelper)
    : IRequestHandler<SendFileCommand, SendFileResponse>
{
    public async Task<SendFileResponse> Handle(SendFileCommand request, CancellationToken ct)
    {
        var conversation = await uow.Conversations.GetByIdAsync(request.ConversationId)
            ?? throw new KeyNotFoundException($"Conversation {request.ConversationId} not found.");

        string relativePath;
        FileType fileType;

        if (request.File.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            relativePath = await imageHelper.SaveImageAsync(request.File, "messages");
            fileType = request.IsCamera ? FileType.Camera : FileType.Image;
        }
        else
        {
            relativePath = await fileHelper.SaveFileAsync(request.File, "messages");
            fileType = FileType.Pdf;
        }

        var now = DateTime.UtcNow;
        var message = new Message
        {
            Id             = Guid.NewGuid(),
            ConversationId = request.ConversationId,
            SenderId       = request.SenderId,
            Content        = request.Content,
            FileUrl        = relativePath,
            FileType       = fileType,
            IsRead         = false,
            SentAt         = now,
            CreatedAt      = now,
            UpdatedAt      = now
        };

        await uow.Messages.AddAsync(message);

        conversation.UpdatedAt = now;
        uow.Conversations.Update(conversation);

        await uow.SaveChangesAsync();

        var fullFileUrl = fileType == FileType.Image || fileType == FileType.Camera
            ? imageHelper.GetImageUrl(relativePath)
            : fileHelper.GetFileUrl(relativePath);

        return new SendFileResponse(
            message.Id,
            conversation.Id,
            fullFileUrl,
            fileType.ToString(),
            message.Content,
            message.SentAt
        );
    }
}
