using FluentValidation;

namespace MessageService.Features.Conversations.SendFile;

public class SendFileValidator : AbstractValidator<SendFileCommand>
{
    public SendFileValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty();
        RuleFor(x => x.SenderId).NotEmpty();
        RuleFor(x => x.File).NotNull().WithMessage("A file must be provided.");
        RuleFor(x => x.File.Length)
            .LessThanOrEqualTo(50L * 1024 * 1024)
            .When(x => x.File != null)
            .WithMessage("File must not exceed 50 MB.");
    }
}
