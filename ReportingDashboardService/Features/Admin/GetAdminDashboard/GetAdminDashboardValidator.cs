using FluentValidation;

namespace ReportingDashboardService.Features.Admin.GetAdminDashboard
{
    public class GetAdminDashboardValidator : AbstractValidator<GetAdminDashboardQuery>
    {
        public GetAdminDashboardValidator()
        {
            // No parameters to validate for the dashboard query
        }
    }
}
