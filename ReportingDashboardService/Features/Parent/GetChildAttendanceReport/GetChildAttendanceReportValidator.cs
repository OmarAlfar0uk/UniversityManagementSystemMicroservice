using FluentValidation;

namespace ReportingDashboardService.Features.Parent.GetChildAttendanceReport
{
    public class GetChildAttendanceReportValidator : AbstractValidator<GetChildAttendanceReportQuery>
    {
        public GetChildAttendanceReportValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("StudentId is required.");
        }
    }
}
