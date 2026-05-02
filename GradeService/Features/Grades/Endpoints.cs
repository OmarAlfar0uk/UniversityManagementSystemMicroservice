using System.Security.Claims;
using GradeService.Features.Grades.GetFinalGrade;
using GradeService.Features.Grades.GetGPA;
using GradeService.Features.Grades.GetMidtermGrade;
using GradeService.Features.Grades.SetFinalGrade;
using GradeService.Features.Grades.SetMidtermGrade;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using GradeService.Contracts;
using System.Net.Http.Json;
using System.Net.Http.Headers;

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
                        .RequireAuthorization(p => p.RequireRole("Admin", "SuperAdmin", "Doctor"))
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

        doctor.MapGet("/{courseId:guid}/students", async (
            Guid courseId,
            IUnitOfWork uow,
            IHttpClientFactory httpClientFactory,
            HttpContext httpContext) =>
        {
            var client = httpClientFactory.CreateClient("AuthService");
            var incomingAuth = httpContext.Request.Headers.Authorization.ToString();
            if (!string.IsNullOrWhiteSpace(incomingAuth))
            {
                client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(incomingAuth);
            }
            var grades = (await uow.StudentGrades.GetAllAsync(g => g.CourseId == courseId)).ToList();

            var students = new List<object>(grades.Count);
            foreach (var grade in grades)
            {
                string name = string.Empty;
                try
                {
                    var response = await client.GetAsync($"/api/v1/auth/internal/users/{grade.StudentId}");
                    if (response.IsSuccessStatusCode)
                    {
                        var userInfo = await response.Content.ReadFromJsonAsync<AuthUserResponse>();
                        name = userInfo?.fullName ?? string.Empty;
                    }
                }
                catch
                {
                    // If Auth service is unreachable, return grades with empty name.
                }

                students.Add(new
                {
                    id = grade.StudentId,
                    name,
                    grades = new
                    {
                        attendance = grade.AttendanceScore,
                        assignment = grade.AssignmentScore,
                        quiz = grade.QuizScore,
                        midterm = grade.MidtermScore,
                        final = grade.FinalScore,
                        total = grade.TotalScore
                    }
                });
            }

            return Results.Ok(students);
        }).WithSummary("Get students with current grades by course (Doctor)");
    }
}

public record GradeBody(decimal Score);
file record AuthUserResponse(Guid id, string fullName);
