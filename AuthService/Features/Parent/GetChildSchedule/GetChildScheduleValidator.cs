using FluentValidation;

namespace AuthService.Features.Parent.GetChildSchedule
{
    public class GetChildScheduleValidator : AbstractValidator<GetChildScheduleQuery>
    {
        public GetChildScheduleValidator()
        {
            RuleFor(x => x.ParentId)
                .NotEmpty().WithMessage("ParentId is required.");
        }
    }
}
