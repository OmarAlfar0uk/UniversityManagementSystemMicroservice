using FluentValidation;

namespace AuthService.Features.Auth.ForgotPassword
{
    public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(x => x.EmailOrId)
                .NotEmpty().WithMessage("Email or University ID is required.");
        }
    }
}
