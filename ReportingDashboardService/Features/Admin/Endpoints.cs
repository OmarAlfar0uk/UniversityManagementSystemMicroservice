using MediatR;
using ReportingDashboardService.Features.Admin.GetAdminDashboard;
using ReportingDashboardService.Features.Admin.GetStudentReport;
using ReportingDashboardService.Features.Admin.GetCourseStats;

namespace ReportingDashboardService.Features.Admin
{
    public static class AdminEndpoints
    {
        public static void MapAdminReportEndpoints(this WebApplication app)
        {
            // GET /api/v1/report/admin/dashboard
            app.MapGet("/api/v1/report/admin/dashboard",
                async (IMediator mediator) =>
                {
                    var result = await mediator.Send(new GetAdminDashboardQuery());
                    return Results.Ok(result);
                })
                .WithName("GetAdminDashboard")
                .RequireAuthorization()
                .WithTags("Admin Reports");

            // GET /api/v1/report/admin/students/{studentId}
            app.MapGet("/api/v1/report/admin/students/{studentId:guid}",
                async (Guid studentId, IMediator mediator) =>
                {
                    var result = await mediator.Send(new GetStudentReportQuery(studentId));
                    return Results.Ok(result);
                })
                .WithName("GetAdminStudentReport")
                .RequireAuthorization()
                .WithTags("Admin Reports");

            // GET /api/v1/report/admin/courses/{courseId}/stats
            app.MapGet("/api/v1/report/admin/courses/{courseId:guid}/stats",
                async (Guid courseId, IMediator mediator) =>
                {
                    var result = await mediator.Send(new GetCourseStatsQuery(courseId));
                    return Results.Ok(result);
                })
                .WithName("GetAdminCourseStats")
                .RequireAuthorization()
                .WithTags("Admin Reports");
        }
    }
}
