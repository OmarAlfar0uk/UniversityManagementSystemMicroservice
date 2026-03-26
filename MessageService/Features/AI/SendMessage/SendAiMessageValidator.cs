using FluentValidation;

namespace MessageService.Features.AI.SendMessage;

public class SendAiMessageValidator : AbstractValidator<SendAiMessageCommand>
{
    public SendAiMessageValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message cannot be empty.")
            .MaximumLength(4000).WithMessage("Message must not exceed 4000 characters.");
    }
}
