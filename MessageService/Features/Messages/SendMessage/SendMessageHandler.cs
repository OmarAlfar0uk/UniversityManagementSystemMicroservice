using MessageService.Contracts;
using MessageService.Data.Models;
using MediatR;

namespace MessageService.Features.Messages.SendMessage;

public class SendMessageHandler : IRequestHandler<SendMessageCommand, SendMessageResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public SendMessageHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<SendMessageResponse> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        // Find or create conversation
        var conversation = await _unitOfWork.Conversations.FindAsync(
            c => (c.StudentId == request.SenderId && c.DoctorId == request.ReceiverId) ||
                 (c.StudentId == request.ReceiverId && c.DoctorId == request.SenderId));

        if (conversation is null)
        {
            conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                StudentId = request.SenderId,
                DoctorId = request.ReceiverId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Conversations.AddAsync(conversation);
        }
        else
        {
            conversation.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Conversations.Update(conversation);
        }

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            SenderId = request.SenderId,
            Content = request.Content,
            SentAt = DateTime.UtcNow,
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Messages.AddAsync(message);
        await _unitOfWork.SaveChangesAsync();

        return new SendMessageResponse(message.Id, conversation.Id, message.SentAt);
    }
}
