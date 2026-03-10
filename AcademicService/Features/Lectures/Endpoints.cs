using System.Security.Claims;
using AcademicService.Features.Lectures.AddLecture;
using AcademicService.Features.Lectures.DeleteLecture;
using AcademicService.Features.Lectures.GetLectureById;
using AcademicService.Features.Lectures.GetLecturesByCourse;
using AcademicService.Features.Lectures.UpdateLecture;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AcademicService.Features.Lectures;

public static class Endpoints
{
    public static void MapLectureEndpoints(this IEndpointRouteBuilder app)
    {
        var student = app.MapGroup("/api/v1/academic/courses")
                         .RequireAuthorization()
                         .WithTags("Academic – Lectures");

        student.MapGet("/{courseId:guid}/lectures", async (Guid courseId, ISender sender) =>
        {
            var result = await sender.Send(new GetLecturesByCourseQuery(courseId));
            return Results.Ok(result);
        }).WithSummary("Get all lectures in a course (Student)");

        student.MapGet("/{courseId:guid}/lectures/{lectureId:guid}", async (
            Guid courseId,
            Guid lectureId,
            ISender sender) =>
        {
            var result = await sender.Send(new GetLectureByIdQuery(courseId, lectureId));
            return Results.Ok(result);
        }).WithSummary("Get lecture details (Student)");

        var admin = app.MapGroup("/api/v1/academic/admin")
                       .RequireAuthorization()
                       .WithTags("Academic – Lectures (Admin)");

        admin.MapPost("/courses/{courseId:guid}/lectures", async (
            Guid courseId,
            [FromForm] string title,
            [FromForm] int orderIndex,
            IFormFile? thumbnail,
            ISender sender) =>
        {
            var result = await sender.Send(new AddLectureCommand(courseId, title, orderIndex, thumbnail));
            return Results.Created($"/api/v1/academic/courses/{courseId}/lectures/{result.Id}", result);
        })
        .DisableAntiforgery()
        .WithSummary("Add lecture to course (Admin)");

        admin.MapPut("/lectures/{lectureId:guid}", async (
            Guid lectureId,
            [FromForm] string title,
            [FromForm] int orderIndex,
            IFormFile? thumbnail,
            ISender sender) =>
        {
            var result = await sender.Send(new UpdateLectureCommand(lectureId, title, orderIndex, thumbnail));
            return Results.Ok(result);
        })
        .DisableAntiforgery()
        .WithSummary("Update lecture (Admin)");

        admin.MapDelete("/lectures/{lectureId:guid}", async (Guid lectureId, ISender sender) =>
        {
            await sender.Send(new DeleteLectureCommand(lectureId));
            return Results.NoContent();
        }).WithSummary("Delete lecture (Admin)");
    }
}
