using FluentValidation;

namespace MessageService.Features.AI.DeleteHistory;

public class DeleteAiHistoryValidator : AbstractValidator<DeleteAiHistoryCommand>
{
    public DeleteAiHistoryValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();
    }
}
