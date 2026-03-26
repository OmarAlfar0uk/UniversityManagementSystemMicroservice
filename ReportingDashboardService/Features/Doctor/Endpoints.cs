using MediatR;
using ReportingDashboardService.Features.Doctor.GetDoctorDashboard;
using ReportingDashboardService.Features.Doctor.GetCourseDetailStats;

namespace ReportingDashboardService.Features.Doctor
{
    public static class DoctorEndpoints
    {
        public static void MapDoctorReportEndpoints(this WebApplication app)
        {
            // GET /api/v1/report/doctor/dashboard
            app.MapGet("/api/v1/report/doctor/dashboard",
                async (HttpContext httpContext, IMediator mediator) =>
                {
                    var doctorIdClaim = httpContext.User.FindFirst("sub")?.Value
                                     ?? httpContext.User.FindFirst(
                                         System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                    if (doctorIdClaim is null || !Guid.TryParse(doctorIdClaim, out var doctorId))
                        return Results.Unauthorized();

                    var result = await mediator.Send(new GetDoctorDashboardQuery(doctorId));
                    return Results.Ok(result);
                })
                .WithName("GetDoctorDashboard")
                .RequireAuthorization()
                .WithTags("Doctor Reports");

            // GET /api/v1/report/doctor/courses/{courseId}/stats
            app.MapGet("/api/v1/report/doctor/courses/{courseId:guid}/stats",
                async (Guid courseId, IMediator mediator) =>
                {
                    var result = await mediator.Send(new GetCourseDetailStatsQuery(courseId));
                    return Results.Ok(result);
                })
                .WithName("GetDoctorCourseDetailStats")
                .RequireAuthorization()
                .WithTags("Doctor Reports");
        }
    }
}
