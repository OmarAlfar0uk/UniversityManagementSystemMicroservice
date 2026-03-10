using FluentValidation;

namespace NotificationService.Features.Notifications.SendNotification;

public class SendNotificationValidator : AbstractValidator<SendNotificationCommand>
{
    public SendNotificationValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Body).NotEmpty().MaximumLength(500);
    }
}
