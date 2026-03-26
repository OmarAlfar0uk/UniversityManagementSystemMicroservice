using FluentValidation;

namespace MessageService.Features.AI.SendFile;

public class SendAiFileValidator : AbstractValidator<SendAiFileCommand>
{
    public SendAiFileValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.File).NotNull().WithMessage("A file must be provided.");
        RuleFor(x => x.File.Length)
            .LessThanOrEqualTo(20L * 1024 * 1024)
            .When(x => x.File != null)
            .WithMessage("File must not exceed 20 MB.");
    }
}
