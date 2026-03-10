using FluentValidation;

namespace NotificationService.Features.Notifications.MarkNotificationRead;

public class MarkNotificationReadValidator : AbstractValidator<MarkNotificationReadCommand>
{
    public MarkNotificationReadValidator()
    {
        RuleFor(x => x.NotificationId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
