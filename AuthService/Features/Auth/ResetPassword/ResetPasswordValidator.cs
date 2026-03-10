using FluentValidation;

namespace AuthService.Features.Auth.ResetPassword
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.EmailOrId)
                .NotEmpty().WithMessage("Email or University ID is required.");

            RuleFor(x => x.ResetToken)
                .NotEmpty().WithMessage("Reset Token is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New Password is required.")
                .MinimumLength(6).WithMessage("New Password must be at least 6 characters.");
        }
    }
}
