using FluentValidation;

namespace ReportingDashboardService.Features.Admin.GetStudentReport
{
    public class GetStudentReportValidator : AbstractValidator<GetStudentReportQuery>
    {
        public GetStudentReportValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("StudentId is required.");
        }
    }
}
