using AcademicService.Features.Schedule.GetClassSchedule;
using AcademicService.Features.Schedule.GetMidtermSchedule;
using MediatR;

namespace AcademicService.Features.Schedule;

public static class Endpoints
{
    public static void MapScheduleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/academic/schedule")
                       .RequireAuthorization()
                       .WithTags("Academic – Schedule");

        group.MapGet("/", async (ISender sender) =>
        {
            var result = await sender.Send(new GetClassScheduleQuery());
            return Results.Ok(result);
        }).WithSummary("Get class schedule (Student)");

        group.MapGet("/midterm", async (ISender sender) =>
        {
            var result = await sender.Send(new GetMidtermScheduleQuery());
            return Results.Ok(result);
        }).WithSummary("Get midterm schedule (Student)");
    }
}
