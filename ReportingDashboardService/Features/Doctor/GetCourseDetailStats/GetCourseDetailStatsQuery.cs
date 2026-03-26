using MediatR;
using ReportingDashboardService.Responses;

namespace ReportingDashboardService.Features.Doctor.GetCourseDetailStats
{
    // Reuses the shared GetCourseStatsQuery and CourseStatsResponse from Admin
    public record GetCourseDetailStatsQuery(Guid CourseId) : IRequest<CourseStatsResponse>;
}
