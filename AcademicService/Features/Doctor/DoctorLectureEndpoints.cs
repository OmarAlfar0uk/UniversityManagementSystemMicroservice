using System.Security.Claims;
using AcademicService.Features.LectureMaterials.GetLecturePdf;
using AcademicService.Features.LectureMaterials.GetLectureVideo;
using AcademicService.Features.LectureMaterials.UploadLecturePdf;
using AcademicService.Features.LectureMaterials.UploadLectureVideo;
using AcademicService.Features.Assignments.GetAssignment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AcademicService.Features.Doctor
{
    /// <summary>
    /// Doctor-specific endpoints - owned by the Doctor team.
    /// Base route: /api/v1/academic/doctor/lectures
    /// Required roles: Doctor
    /// </summary>
    public static class DoctorLectureEndpoints
    {
        public static WebApplication MapDoctorLectureEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/v1/academic/doctor/lectures")
                           .RequireAuthorization(p => p.RequireRole("Doctor"))
                           .WithTags("Doctor - Lectures");

            // GET /api/v1/academic/doctor/lectures/{lectureId}/pdf
            group.MapGet("{lectureId:guid}/pdf", async (
                Guid lectureId,
                ISender sender) =>
            {
                var result = await sender.Send(new GetLecturePdfQuery(lectureId));
                return Results.Ok(result);
            }).WithSummary("Get PDF for a lecture (Doctor)");

            // PUT /api/v1/academic/doctor/lectures/{lectureId}/pdf
            group.MapPut("{lectureId:guid}/pdf", async (
                Guid lectureId,
                IFormFile file,
                ISender sender) =>
            {
                var command = new UploadLecturePdfCommand(lectureId, file);
                var result = await sender.Send(command);
                return Results.Ok(result);
            }).WithSummary("Upload/Update PDF for a lecture (Doctor)");

            // GET /api/v1/academic/doctor/lectures/{lectureId}/video
            group.MapGet("{lectureId:guid}/video", async (
                Guid lectureId,
                ISender sender) =>
            {
                var result = await sender.Send(new GetLectureVideoQuery(lectureId));
                return Results.Ok(result);
            }).WithSummary("Get video for a lecture (Doctor)");

            // PUT /api/v1/academic/doctor/lectures/{lectureId}/video
            group.MapPut("{lectureId:guid}/video", async (
                Guid lectureId,
                IFormFile file,
                ISender sender) =>
            {
                var command = new UploadLectureVideoCommand(lectureId, file);
                var result = await sender.Send(command);
                return Results.Ok(result);
            }).WithSummary("Upload/Update video for a lecture (Doctor)");

            // GET /api/v1/academic/doctor/lectures/{lectureId}/assignment
            group.MapGet("{lectureId:guid}/assignment", async (
                Guid lectureId,
                ISender sender) =>
            {
                var result = await sender.Send(new GetAssignmentQuery(lectureId));
                return Results.Ok(result);
            }).WithSummary("Get assignment for a lecture (Doctor)");

            return app;
        }
    }
}
