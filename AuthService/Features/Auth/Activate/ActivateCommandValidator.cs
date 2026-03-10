using FluentValidation;

namespace AuthService.Features.Auth.Activate
{
    public class ActivateCommandValidator : AbstractValidator<ActivateCommand>
    {
        public ActivateCommandValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6);
        }
    }
}
