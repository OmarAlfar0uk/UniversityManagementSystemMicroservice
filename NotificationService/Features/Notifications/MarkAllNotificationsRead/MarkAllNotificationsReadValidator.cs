using FluentValidation;

namespace NotificationService.Features.Notifications.MarkAllNotificationsRead;

public class MarkAllNotificationsReadValidator : AbstractValidator<MarkAllNotificationsReadCommand>
{
    public MarkAllNotificationsReadValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
