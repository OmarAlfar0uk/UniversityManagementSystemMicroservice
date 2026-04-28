using AcademicService.Features.Departments.GetDepartmentDetails;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AcademicService.Features.Departments;

public static class Endpoints
{
    public static void MapDepartmentEndpoints(this IEndpointRouteBuilder app)
    {
        var admin = app.MapGroup("/api/v1/academic/admin/departments")
            .RequireAuthorization()
            .WithTags("Academic – Departments (Admin)");

        admin.MapPost("/", async ([FromBody] CreateDepartmentRequest body, ISender sender) =>
        {
            var result = await sender.Send(new CreateDepartmentCommand(body.Name, body.Code));
            return Results.Created($"/api/v1/academic/admin/departments/{result.Id}", result);
        }).WithSummary("Create a department");

        admin.MapGet("/", async (ISender sender) =>
        {
            var result = await sender.Send(new GetDepartmentsQuery());
            return Results.Ok(result);
        }).WithSummary("List departments");

        admin.MapGet("/{departmentId:guid}", async (
            Guid departmentId,
            ISender sender) =>
        {
            var result = await sender.Send(new GetDepartmentDetailsQuery(departmentId));
            return Results.Ok(result);
        }).WithSummary("Get department details with courses and student counts (Admin)");

        admin.MapPost("/{departmentId:guid}/enrollments/sync", async (Guid departmentId, ISender sender) =>
        {
            var result = await sender.Send(new SyncDepartmentEnrollmentsCommand(departmentId));
            return Results.Ok(result);
        }).WithSummary("Sync all department students into their department course offerings");
    }
}

public record CreateDepartmentRequest(string Name, string Code);

