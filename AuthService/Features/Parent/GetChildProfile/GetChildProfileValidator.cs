using FluentValidation;

namespace AuthService.Features.Parent.GetChildProfile
{
    public class GetChildProfileValidator : AbstractValidator<GetChildProfileQuery>
    {
        public GetChildProfileValidator()
        {
            RuleFor(x => x.ParentId)
                .NotEmpty().WithMessage("ParentId is required.");
        }
    }
}
