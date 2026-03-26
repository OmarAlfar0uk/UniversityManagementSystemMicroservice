using FluentValidation;

namespace ReportingDashboardService.Features.Doctor.GetCourseDetailStats
{
    public class GetCourseDetailStatsValidator : AbstractValidator<GetCourseDetailStatsQuery>
    {
        public GetCourseDetailStatsValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("CourseId is required.");
        }
    }
}
