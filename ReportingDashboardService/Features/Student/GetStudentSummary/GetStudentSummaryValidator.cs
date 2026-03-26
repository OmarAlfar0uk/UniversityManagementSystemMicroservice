using FluentValidation;

namespace ReportingDashboardService.Features.Student.GetStudentSummary
{
    public class GetStudentSummaryValidator : AbstractValidator<GetStudentSummaryQuery>
    {
        public GetStudentSummaryValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("StudentId is required.");
        }
    }
}
