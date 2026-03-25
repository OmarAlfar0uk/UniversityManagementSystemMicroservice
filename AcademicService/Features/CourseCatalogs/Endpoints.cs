using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AcademicService.Features.CourseCatalogs;

public static class Endpoints
{
    public static void MapCourseCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var admin = app.MapGroup("/api/v1/academic/admin/course-catalogs")
            .RequireAuthorization()
            .WithTags("Academic – Course Catalogs (Admin)");

        admin.MapPost("/", async (
            [FromForm] string name,
            [FromForm] string code,
            [FromForm] string? description,
            IFormFile? coverImage,
            ISender sender) =>
        {
            var result = await sender.Send(new CreateCourseCatalogCommand(name, code, description, coverImage));
            return Results.Created($"/api/v1/academic/admin/course-catalogs/{result.Id}", result);
        })
        .DisableAntiforgery()
        .WithSummary("Create a course catalog");

        admin.MapGet("/", async (ISender sender) =>
        {
            var result = await sender.Send(new GetCourseCatalogsQuery());
            return Results.Ok(result);
        }).WithSummary("List course catalogs");

        admin.MapPost("/{courseCatalogId:guid}/offerings", async (
            Guid courseCatalogId,
            [FromBody] CreateCourseOfferingRequest body,
            ISender sender) =>
        {
            var result = await sender.Send(new CreateCourseOfferingCommand(
                courseCatalogId,
                body.DepartmentId,
                body.DoctorId));
            return Results.Created($"/api/v1/academic/courses/{result.Id}", result);
        }).WithSummary("Create a department offering from a course catalog");
    }
}

public record CreateCourseOfferingRequest(Guid DepartmentId, Guid DoctorId);
