using MediatR;
using ReportingDashboardService.Responses;

namespace ReportingDashboardService.Features.Student.GetStudentSummary
{
    public record GetStudentSummaryQuery(Guid StudentId) : IRequest<StudentSummaryResponse>;
}
