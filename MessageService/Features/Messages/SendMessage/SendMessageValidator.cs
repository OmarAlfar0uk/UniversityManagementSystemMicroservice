using FluentValidation;

namespace MessageService.Features.Messages.SendMessage;

public class SendMessageValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageValidator()
    {
        RuleFor(x => x.SenderId).NotEmpty();
        RuleFor(x => x.ReceiverId).NotEmpty();
        RuleFor(x => x.Content).NotEmpty().MaximumLength(2000);
    }
}
