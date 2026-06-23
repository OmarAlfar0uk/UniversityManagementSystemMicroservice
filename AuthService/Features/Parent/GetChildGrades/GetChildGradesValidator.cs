using FluentValidation;

namespace AuthService.Features.Parent.GetChildGrades
{
    public class GetChildGradesValidator : AbstractValidator<GetChildGradesQuery>
    {
        public GetChildGradesValidator()
        {
            RuleFor(x => x.ParentId)
                .NotEmpty().WithMessage("ParentId is required.");
        }
    }
}
