using MediatR;
using ReportingDashboardService.Responses;

namespace ReportingDashboardService.Features.Admin.GetStudentReport
{
    public record GetStudentReportQuery(Guid StudentId) : IRequest<StudentSummaryResponse>;
}
