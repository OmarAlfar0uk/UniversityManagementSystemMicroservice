using FluentValidation;

namespace AuthService.Features.Auth.ResendOtp
{
    public class ResendOtpValidator : AbstractValidator<ResendOtpCommand>
    {
        public ResendOtpValidator()
        {
            RuleFor(x => x.EmailOrId)
                .NotEmpty().WithMessage("Email or University ID is required.");
        }
    }
}
