using System.Security.Claims;
using ProgressService.Features.Progress.GetCourseProgress;
using ProgressService.Features.Progress.GetLectureProgress;
using ProgressService.Features.Progress.UpdateProgress;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ProgressService.Features.Progress;

public static class Endpoints
{
    public static void MapProgressEndpoints(this IEndpointRouteBuilder app)
    {
        var student = app.MapGroup("/api/v1/progress")
                         .RequireAuthorization()
                         .WithTags("Progress – Student");

        student.MapGet("/courses/{courseId:guid}", async (
            Guid courseId, ISender sender, ClaimsPrincipal user) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await sender.Send(new GetCourseProgressQuery(courseId, studentId));
            return Results.Ok(result);
        }).WithSummary("Get course progress (Student)");

        student.MapGet("/lectures/{lectureId:guid}", async (
            Guid lectureId, ISender sender, ClaimsPrincipal user) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await sender.Send(new GetLectureProgressQuery(lectureId, studentId));
            return Results.Ok(result);
        }).WithSummary("Get lecture progress for current student");

        // Internal endpoint — called by AcademicService when video is watched
        app.MapPost("/api/v1/progress/internal/update", async (
            [FromBody] UpdateProgressCommand command,
            ISender sender) =>
        {
            await sender.Send(command);
            return Results.Ok(new { message = "Progress updated." });
        }).RequireAuthorization().WithTags("Progress – Internal").WithSummary("Update progress after video watched (internal)");
    }
}
