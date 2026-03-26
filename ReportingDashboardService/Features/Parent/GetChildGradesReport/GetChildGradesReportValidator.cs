using FluentValidation;

namespace ReportingDashboardService.Features.Parent.GetChildGradesReport
{
    public class GetChildGradesReportValidator : AbstractValidator<GetChildGradesReportQuery>
    {
        public GetChildGradesReportValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("StudentId is required.");
        }
    }
}
