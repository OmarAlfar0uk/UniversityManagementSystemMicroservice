using FluentValidation;

namespace MessageService.Features.AI.AskRashed;

public class AskRashedValidator : AbstractValidator<AskRashedCommand>
{
    public AskRashedValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message cannot be empty.")
            .MaximumLength(2000).WithMessage("Message cannot exceed 2000 characters.");
    }
}
