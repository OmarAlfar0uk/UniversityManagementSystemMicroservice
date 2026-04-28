using System.Security.Claims;
using AcademicService.Features.Schedule.GetClassSchedule;
using AcademicService.Features.Schedule.GetMidtermSchedule;
using AcademicService.Features.Schedule.UploadSchedule;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AcademicService.Features.Schedule;

public static class Endpoints
{
    public static void MapScheduleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/academic")
                       .WithTags("Academic – Schedule");

        // GET /api/v1/academic/schedule
        group.MapGet("/schedule", async (
            ISender sender,
            ClaimsPrincipal user) =>
        {
            var departmentId = Guid.Parse(
                user.FindFirstValue("departmentId")
                ?? user.FindFirstValue("DepartmentId")
                ?? throw new UnauthorizedAccessException("DepartmentId claim missing."));

            var result = await sender.Send(new GetClassScheduleQuery(departmentId));
            return Results.Ok(result);
        })
        .RequireAuthorization()
        .WithSummary("Get class schedule for student's department");

        // GET /api/v1/academic/schedule/midterm
        group.MapGet("/schedule/midterm", async (
            ISender sender,
            ClaimsPrincipal user) =>
        {
            var departmentId = Guid.Parse(
                user.FindFirstValue("departmentId")
                ?? user.FindFirstValue("DepartmentId")
                ?? throw new UnauthorizedAccessException("DepartmentId claim missing."));

            var result = await sender.Send(new GetMidtermScheduleQuery(departmentId));
            return Results.Ok(result);
        })
        .RequireAuthorization()
        .WithSummary("Get midterm schedule for student's department");

        // POST /api/v1/academic/admin/schedule
        var adminGroup = app.MapGroup("/api/v1/academic/admin")
                            .RequireAuthorization(p => p.RequireRole("Admin", "SuperAdmin"))
                            .WithTags("Academic – Schedule (Admin)");

        adminGroup.MapPost("/schedule", async (
            ISender sender,
            [FromForm] string type,
            [FromForm] Guid departmentId,
            [FromForm] string? academicYear,
            IFormFile image) =>
        {
            var command = new UploadScheduleCommand(image, type, departmentId, academicYear);
            var result = await sender.Send(command);
            return Results.Ok(result);
        })
        .DisableAntiforgery()
        .WithSummary("Upload schedule image for a department (Admin)");
    }
}
