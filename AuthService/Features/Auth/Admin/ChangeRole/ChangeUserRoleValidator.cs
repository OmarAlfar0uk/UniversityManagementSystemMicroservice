using FluentValidation;

namespace AuthService.Features.Auth.Admin.ChangeRole
{
    public class ChangeUserRoleValidator
        : AbstractValidator<ChangeUserRoleCommand>
    {
        public ChangeUserRoleValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.NewRole).NotEmpty();
        }
    }
}
