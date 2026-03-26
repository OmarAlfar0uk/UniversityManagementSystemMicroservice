using FluentValidation;

namespace ReportingDashboardService.Features.Admin.GetCourseStats
{
    public class GetCourseStatsValidator : AbstractValidator<GetCourseStatsQuery>
    {
        public GetCourseStatsValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("CourseId is required.");
        }
    }
}
