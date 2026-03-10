using System.Security.Claims;
using GradeService.Features.Grades.GetFinalGrade;
using GradeService.Features.Grades.GetGPA;
using GradeService.Features.Grades.GetMidtermGrade;
using GradeService.Features.Grades.SetFinalGrade;
using GradeService.Features.Grades.SetMidtermGrade;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GradeService.Features.Grades;

public static class Endpoints
{
    public static void MapGradeEndpoints(this IEndpointRouteBuilder app)
    {
        var student = app.MapGroup("/api/v1/grade/courses")
                         .RequireAuthorization()
                         .WithTags("Grade – Student");

        student.MapGet("/{courseId:guid}/midterm", async (
            Guid courseId, ISender sender, ClaimsPrincipal user) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await sender.Send(new GetMidtermGradeQuery(courseId, studentId));
            return Results.Ok(result);
        }).WithSummary("Get midterm grade (Student)");

        student.MapGet("/{courseId:guid}/final", async (
            Guid courseId, ISender sender, ClaimsPrincipal user) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await sender.Send(new GetFinalGradeQuery(courseId, studentId));
            return Results.Ok(result);
        }).WithSummary("Get final grade (Student)");

        app.MapGet("/api/v1/grade/gpa", async (ISender sender, ClaimsPrincipal user) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await sender.Send(new GetGPAQuery(studentId));
            return Results.Ok(result);
        }).RequireAuthorization().WithTags("Grade – Student").WithSummary("Get GPA (Student)");

        var doctor = app.MapGroup("/api/v1/grade/admin/courses")
                        .RequireAuthorization()
                        .WithTags("Grade – Doctor/Admin");

        doctor.MapPut("/{courseId:guid}/students/{studentId:guid}/midterm", async (
            Guid courseId, Guid studentId,
            [FromBody] GradeBody body, ISender sender) =>
        {
            var result = await sender.Send(new SetMidtermGradeCommand(courseId, studentId, body.Score));
            return Results.Ok(result);
        }).WithSummary("Set midterm grade (Doctor)");

        doctor.MapPut("/{courseId:guid}/students/{studentId:guid}/final", async (
            Guid courseId, Guid studentId,
            [FromBody] GradeBody body, ISender sender) =>
        {
            var result = await sender.Send(new SetFinalGradeCommand(courseId, studentId, body.Score));
            return Results.Ok(result);
        }).WithSummary("Set final grade (Doctor)");
    }
}

public record GradeBody(decimal Score);
