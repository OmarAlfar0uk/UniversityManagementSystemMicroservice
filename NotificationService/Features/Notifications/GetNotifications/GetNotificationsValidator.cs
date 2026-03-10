using FluentValidation;

namespace NotificationService.Features.Notifications.GetNotifications;

public class GetNotificationsValidator : AbstractValidator<GetNotificationsQuery>
{
    public GetNotificationsValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
