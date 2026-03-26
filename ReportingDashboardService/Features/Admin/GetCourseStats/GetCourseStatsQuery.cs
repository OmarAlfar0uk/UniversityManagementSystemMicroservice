using MediatR;
using ReportingDashboardService.Responses;

namespace ReportingDashboardService.Features.Admin.GetCourseStats
{
    public record GetCourseStatsQuery(Guid CourseId) : IRequest<CourseStatsResponse>;
}
