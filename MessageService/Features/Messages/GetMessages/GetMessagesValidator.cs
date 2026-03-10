using FluentValidation;

namespace MessageService.Features.Messages.GetMessages;

public class GetMessagesValidator : AbstractValidator<GetMessagesQuery>
{
    public GetMessagesValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty();
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
