using System.Security.Claims;
using ProgressService.Features.Progress.GetCourseProgress;
using ProgressService.Features.Progress.GetLectureProgress;
using ProgressService.Features.Progress.UpdateProgress;
using ProgressService.Features.Progress.MarkPdfViewed;
using ProgressService.Features.Progress.MarkVideoWatched;
using ProgressService.Features.Progress.SyncAttendance;
using ProgressService.Features.Progress.SyncAssignment;
using ProgressService.Features.Progress.SyncQuiz;
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

        // GET /api/v1/progress/courses/{courseId}
        student.MapGet("/courses/{courseId:guid}", async (
            Guid courseId, ISender sender, ClaimsPrincipal user) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await sender.Send(new GetCourseProgressQuery(courseId, studentId));
            return Results.Ok(result);
        }).WithSummary("Get course progress (Student)");

        // GET /api/v1/progress/lectures/{lectureId}
        student.MapGet("/lectures/{lectureId:guid}", async (
            Guid lectureId, ISender sender, ClaimsPrincipal user) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await sender.Send(new GetLectureProgressQuery(lectureId, studentId));
            return Results.Ok(result);
        }).WithSummary("Get lecture progress for current student");

        // PUT /api/v1/progress/lectures/{lectureId}/pdf
        student.MapPut("/lectures/{lectureId:guid}/pdf", async (
            Guid lectureId,
            [FromBody] MarkPdfBody body,
            ISender sender,
            ClaimsPrincipal user) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await sender.Send(new MarkPdfViewedCommand(lectureId, body.CourseId, studentId));
            return Results.Ok(result);
        }).WithSummary("Mark PDF as viewed (Student)");

        // PUT /api/v1/progress/lectures/{lectureId}/video
        student.MapPut("/lectures/{lectureId:guid}/video", async (
            Guid lectureId,
            [FromBody] MarkVideoBody body,
            ISender sender,
            ClaimsPrincipal user) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await sender.Send(new MarkVideoWatchedCommand(lectureId, body.CourseId, studentId));
            return Results.Ok(result);
        }).WithSummary("Mark video as watched (Student)");

        // ── Internal endpoints ─────────────────────────────────────────────────
        var internalGroup = app.MapGroup("/api/v1/progress")
                               .WithTags("Progress – Internal");

        // Internal: POST /api/v1/progress/internal/update (existing — called by AcademicService)
        internalGroup.MapPost("/internal/update", async (
            [FromBody] UpdateProgressCommand command,
            ISender sender) =>
        {
            await sender.Send(command);
            return Results.Ok(new { message = "Progress updated." });
        }).RequireAuthorization().WithSummary("Update progress after video watched (internal)");

        // PUT /api/v1/progress/lectures/{lectureId}/attendance
        internalGroup.MapPut("/lectures/{lectureId:guid}/attendance", async (
            Guid lectureId,
            [FromBody] InternalSyncBody body,
            ISender sender) =>
        {
            var result = await sender.Send(new SyncAttendanceCommand(lectureId, body.CourseId, body.StudentId));
            return Results.Ok(result);
        }).WithSummary("Sync attendance progress (internal — AttendanceService)");

        // PUT /api/v1/progress/lectures/{lectureId}/assignment
        internalGroup.MapPut("/lectures/{lectureId:guid}/assignment", async (
            Guid lectureId,
            [FromBody] InternalSyncBody body,
            ISender sender) =>
        {
            var result = await sender.Send(new SyncAssignmentCommand(lectureId, body.CourseId, body.StudentId));
            return Results.Ok(result);
        }).WithSummary("Sync assignment progress (internal — AcademicService)");

        // PUT /api/v1/progress/lectures/{lectureId}/quiz
        internalGroup.MapPut("/lectures/{lectureId:guid}/quiz", async (
            Guid lectureId,
            [FromBody] InternalSyncBody body,
            ISender sender) =>
        {
            var result = await sender.Send(new SyncQuizCommand(lectureId, body.CourseId, body.StudentId));
            return Results.Ok(result);
        }).WithSummary("Sync quiz progress (internal — ExamService)");
    }
}

public record MarkPdfBody(Guid CourseId);
public record MarkVideoBody(Guid CourseId);
public record InternalSyncBody(Guid CourseId, Guid StudentId);

