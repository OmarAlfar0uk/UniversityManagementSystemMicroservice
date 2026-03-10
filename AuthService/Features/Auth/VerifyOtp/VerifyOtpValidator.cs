using FluentValidation;

namespace AuthService.Features.Auth.VerifyOtp
{
    public class VerifyOtpValidator : AbstractValidator<VerifyOtpCommand>
    {
        public VerifyOtpValidator()
        {
            RuleFor(x => x.EmailOrId)
                .NotEmpty().WithMessage("Email or University ID is required.");
            
            RuleFor(x => x.Otp)
                .NotEmpty().WithMessage("OTP is required.")
                .Length(6).WithMessage("OTP must be exactly 6 characters.");
        }
    }
}
