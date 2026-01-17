using FluentValidation;

namespace AuthService.Features.Auth.Parent.ActivateParent
{
    public class ActivateParentValidator : AbstractValidator<ActivateParentCommand>
    {
        public ActivateParentValidator()
        {
            RuleFor(x => x.Code).NotEmpty();
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Password).MinimumLength(6);
        }
    }
}
