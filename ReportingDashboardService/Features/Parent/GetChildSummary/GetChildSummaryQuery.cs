using MediatR;
using ReportingDashboardService.Responses;

namespace ReportingDashboardService.Features.Parent.GetChildSummary
{
    public record GetChildSummaryQuery(Guid StudentId) : IRequest<StudentSummaryResponse>;
}
