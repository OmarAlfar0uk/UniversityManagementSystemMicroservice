using FluentValidation;

namespace MessageService.Features.Messages.GetConversations;

public class GetConversationsValidator : AbstractValidator<GetConversationsQuery>
{
    public GetConversationsValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
