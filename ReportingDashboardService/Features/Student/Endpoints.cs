using MediatR;
using ReportingDashboardService.Features.Student.GetStudentSummary;

namespace ReportingDashboardService.Features.Student
{
    public static class StudentEndpoints
    {
        public static void MapStudentReportEndpoints(this WebApplication app)
        {
            app.MapGet("/api/v1/report/student/summary",
                async (HttpContext httpContext, IMediator mediator) =>
                {
                    var userIdClaim = httpContext.User.FindFirst("sub")?.Value
                                   ?? httpContext.User.FindFirst(
                                       System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                    if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var studentId))
                        return Results.Unauthorized();

                    var result = await mediator.Send(new GetStudentSummaryQuery(studentId));
                    return Results.Ok(result);
                })
                .WithName("GetStudentSummary")
                .RequireAuthorization()
                .WithTags("Student Reports");
        }
    }
}
