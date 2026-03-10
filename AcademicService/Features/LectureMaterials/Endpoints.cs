using System.Security.Claims;
using AcademicService.Features.LectureMaterials.GetLecturePdf;
using AcademicService.Features.LectureMaterials.GetLectureVideo;
using AcademicService.Features.LectureMaterials.MarkVideoWatched;
using AcademicService.Features.LectureMaterials.UploadLecturePdf;
using AcademicService.Features.LectureMaterials.UploadLectureVideo;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AcademicService.Features.LectureMaterials;

public static class Endpoints
{
    public static void MapLectureMaterialEndpoints(this IEndpointRouteBuilder app)
    {
        var student = app.MapGroup("/api/v1/academic/lectures")
                         .RequireAuthorization()
                         .WithTags("Academic – Lecture Materials");

        student.MapGet("/{lectureId:guid}/pdf", async (Guid lectureId, ISender sender) =>
        {
            var result = await sender.Send(new GetLecturePdfQuery(lectureId));
            return Results.Ok(result);
        }).WithSummary("Get lecture PDF (Student)");

        student.MapGet("/{lectureId:guid}/video", async (Guid lectureId, ISender sender) =>
        {
            var result = await sender.Send(new GetLectureVideoQuery(lectureId));
            return Results.Ok(result);
        }).WithSummary("Get lecture video (Student)");

        student.MapPut("/{lectureId:guid}/video/viewed", async (
            Guid lectureId,
            ISender sender,
            ClaimsPrincipal user) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await sender.Send(new MarkVideoWatchedCommand(lectureId, studentId));
            return Results.NoContent();
        }).WithSummary("Mark video as watched (Student)");

        var admin = app.MapGroup("/api/v1/academic/admin/lectures")
                       .RequireAuthorization()
                       .WithTags("Academic – Lecture Materials (Admin)");

        admin.MapPost("/{lectureId:guid}/pdf", async (
            Guid lectureId,
            IFormFile file,
            ISender sender) =>
        {
            var result = await sender.Send(new UploadLecturePdfCommand(lectureId, file));
            return Results.Ok(result);
        })
        .DisableAntiforgery()
        .WithSummary("Upload lecture PDF (Admin/Doctor)");

        admin.MapPost("/{lectureId:guid}/video", async (
            Guid lectureId,
            IFormFile video,
            ISender sender) =>
        {
            var result = await sender.Send(new UploadLectureVideoCommand(lectureId, video));
            return Results.Ok(result);
        })
        .DisableAntiforgery()
        .WithSummary("Upload lecture video (Admin/Doctor)");
    }
}

