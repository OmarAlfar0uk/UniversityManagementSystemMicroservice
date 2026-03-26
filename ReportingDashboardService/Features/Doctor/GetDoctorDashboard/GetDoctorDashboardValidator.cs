using FluentValidation;

namespace ReportingDashboardService.Features.Doctor.GetDoctorDashboard
{
    public class GetDoctorDashboardValidator : AbstractValidator<GetDoctorDashboardQuery>
    {
        public GetDoctorDashboardValidator()
        {
            RuleFor(x => x.DoctorId)
                .NotEmpty().WithMessage("DoctorId is required.");
        }
    }
}
