using MediatR;
using ReportingDashboardService.Features.Parent.GetChildSummary;
using ReportingDashboardService.Features.Parent.GetChildAttendanceReport;
using ReportingDashboardService.Features.Parent.GetChildGradesReport;

namespace ReportingDashboardService.Features.Parent
{
    public static class ParentEndpoints
    {
        public static void MapParentReportEndpoints(this WebApplication app)
        {
            // GET /api/v1/report/parent/children/{studentId}/summary
            app.MapGet("/api/v1/report/parent/children/{studentId:guid}/summary",
                async (Guid studentId, IMediator mediator) =>
                {
                    var result = await mediator.Send(new GetChildSummaryQuery(studentId));
                    return Results.Ok(result);
                })
                .WithName("GetChildSummary")
                .RequireAuthorization()
                .WithTags("Parent Reports");

            // GET /api/v1/report/parent/children/{studentId}/attendance
            app.MapGet("/api/v1/report/parent/children/{studentId:guid}/attendance",
                async (Guid studentId, IMediator mediator) =>
                {
                    var result = await mediator.Send(new GetChildAttendanceReportQuery(studentId));
                    return Results.Ok(result);
                })
                .WithName("GetChildAttendanceReport")
                .RequireAuthorization()
                .WithTags("Parent Reports");

            // GET /api/v1/report/parent/children/{studentId}/grades
            app.MapGet("/api/v1/report/parent/children/{studentId:guid}/grades",
                async (Guid studentId, IMediator mediator) =>
                {
                    var result = await mediator.Send(new GetChildGradesReportQuery(studentId));
                    return Results.Ok(result);
                })
                .WithName("GetChildGradesReport")
                .RequireAuthorization()
                .WithTags("Parent Reports");
        }
    }
}
