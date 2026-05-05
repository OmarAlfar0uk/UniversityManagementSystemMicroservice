using System.Security.Claims;
using AcademicService.Features.LectureMaterials;
using AcademicService.Features.LectureMaterials.GetLecturePdf;
using AcademicService.Features.LectureMaterials.GetLectureVideo;
using AcademicService.Features.LectureMaterials.UploadLecturePdf;
using AcademicService.Features.LectureMaterials.UploadLectureVideo;
using AcademicService.Features.Assignments;
using AcademicService.Features.Assignments.CreateAssignment;
using AcademicService.Features.Assignments.UpdateAssignment;
using AcademicService.Features.Assignments.DeleteAssignment;
using AcademicService.Features.Assignments.GetAssignment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AcademicService.Features.Doctor
{
    /// <summary>
    /// Doctor-specific endpoints — owned by the Doctor team.
    /// Base route: /api/v1/academic/doctor
    /// Required roles: Doctor
    /// </summary>
    public static class DoctorLectureEndpoints
    {
        public static void MapDoctorLectureEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/academic/doctor/lectures")
                           .RequireAuthorization(p => p.RequireRole("Doctor"))
                           .WithTags("Doctor – Lectures");

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
                return results.Ok(result);
            }).WithSummary("Upload/Update PDF for a lecture (Doctor)");

            // DELETE /api/v1/academic/doctor/lectures/{lectureId}/pdf
            group.MapDelete("{lectureId:guid}/pdf", async (
                Guid lectureId,
                ISender sender) =>
            {
                await sender.Send(new DeleteLecturePdfCommand(lectureId));
                return Results.NoContent();
            }).WithSummary("Delete PDF for a lecture (Doctor)");

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

            // DELETE /api/v1/academic/doctor/lectures/{lectureId}/video
            group.MapDelete("{lectureId:guid}/video", async (
                Guid lectureId,
                ISender sender) =>
            {
                await sender.Send(new DeleteLectureVideoCommand(lectureId));
                return Results.NoContent();
            }).WithSummary("Delete video for a lecture (Doctor)");

            // POST /api/v1/academic/doctor/lectures/{lectureId}/assignment
            group.MapPost("{lectureId:guid}/assignment", async (
                Guid lectureId,
                [FromForm] string title,
                IFormFile file,
                ISender sender) =>
            {
                var command = new CreateAssignmentCommand(lectureId, title, file);
                var result = await sender.Send(command);
                return Results.Created($"/api/v1/academic/lectures/{lectureId}/assignment", result);
            }).WithSummary("Create assignment for a lecture (Doctor)");

            // PUT /api/v1/academic/doctor/lectures/{lectureId}/assignment
            group.MapPut("{lectureId:guid}/assignment", async (
                Guid lectureId,
                [FromForm] string? title,
                IFormFile? file,
                ISender sender) =>
            {
                var command = new UpdateAssignmentCommand(lectureId, title, file);
                var result = await sender.Send(command);
                return Results.Ok(result);
            }).WithSummary("Update assignment for a lecture (Doctor)");

            // DELETE /api/v1/academic/doctor/lectures/{lectureId}/assignment
            group.MapDelete("{lectureId:guid}/assignment", async (
                Guid lectureId,
                ISender sender) =>
            {
                await sender.Send(new DeleteAssignmentCommand(lectureId));
                return Results.NoContent();
            }).WithSummary("Delete assignment for a lecture (Doctor)");
        }
    }
}
