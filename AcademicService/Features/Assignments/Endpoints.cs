using System.Security.Claims;
using AcademicService.Features.Assignments.GetAssignment;
using AcademicService.Features.Assignments.GetAssignmentStatus;
using AcademicService.Features.Assignments.SubmitAssignmentFile;
using AcademicService.Features.Assignments.SubmitAssignmentUrl;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AcademicService.Features.Assignments;

public static class Endpoints
{
    public static void MapAssignmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/academic/lectures")
                       .RequireAuthorization()
                       .WithTags("Academic – Assignments");

        group.MapGet("/{lectureId:guid}/assignment", async (Guid lectureId, ISender sender) =>
        {
            var result = await sender.Send(new GetAssignmentQuery(lectureId));
            return Results.Ok(result);
        }).WithSummary("Get assignment for a lecture (Student)");

        group.MapGet("/{lectureId:guid}/assignment/status", async (
            Guid lectureId,
            ISender sender,
            ClaimsPrincipal user) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await sender.Send(new GetAssignmentStatusQuery(lectureId, studentId));
            return Results.Ok(result);
        }).WithSummary("Get assignment submission status (Student)");

        group.MapPost("/{lectureId:guid}/assignment/submit", async (
            Guid lectureId,
            IFormFile file,
            ISender sender,
            ClaimsPrincipal user) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await sender.Send(new SubmitAssignmentFileCommand(lectureId, studentId, file));
            return Results.Ok(new { message = "Assignment submitted successfully." });
        })
        .DisableAntiforgery()
        .WithSummary("Submit assignment as file (Student)");

        group.MapPost("/{lectureId:guid}/assignment/submit-url", async (
            Guid lectureId,
            [FromBody] SubmitUrlBody body,
            ISender sender,
            ClaimsPrincipal user) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await sender.Send(new SubmitAssignmentUrlCommand(lectureId, studentId, body.ProjectUrl));
            return Results.Ok(new { message = "Assignment URL submitted successfully." });
        }).WithSummary("Submit assignment as project URL (Student)");
    }
}


public record SubmitUrlBody(string ProjectUrl);
