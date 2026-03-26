using FluentValidation;

namespace ReportingDashboardService.Features.Parent.GetChildSummary
{
    public class GetChildSummaryValidator : AbstractValidator<GetChildSummaryQuery>
    {
        public GetChildSummaryValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("StudentId is required.");
        }
    }
}
