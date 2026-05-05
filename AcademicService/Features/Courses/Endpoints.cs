using System.Security.Claims;
using AcademicService.Features.Courses.CreateCourse;
using AcademicService.Features.Courses.DeleteCourse;
using AcademicService.Features.Courses.EnrollStudents;
using AcademicService.Features.Courses.GetAllCourses;
using AcademicService.Features.Courses.GetCourseById;
using AcademicService.Features.Courses.GetCourseInstructor;
using AcademicService.Features.Courses.UpdateCourse;
using AcademicService.Features.Courses.GetAllCoursesAdmin;
using AcademicService.Features.Courses.GetCourseStudentsDetails;
using AcademicService.Features.Courses.GetCourseLecturesAdmin;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AcademicService.Features.Courses;

public static class Endpoints
{
    public static void MapCourseEndpoints(this IEndpointRouteBuilder app)
    {
        // Student course endpoints
        var student = app.MapGroup("/api/v1/academic/courses")
                         .RequireAuthorization()
                         .WithTags("Academic – Courses");

        student.MapGet("/", async (
            ISender sender,
            ClaimsPrincipal user,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await sender.Send(new GetAllCoursesQuery(studentId, pageNumber, pageSize));
            return Results.Ok(result);
        }).WithSummary("Get all enrolled courses (Student)");

        student.MapGet("/{courseId:guid}", async (
            Guid courseId,
            ISender sender,
            ClaimsPrincipal user) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await sender.Send(new GetCourseByIdQuery(courseId, studentId));
            return Results.Ok(result);
        }).WithSummary("Get course details (Student)");

        student.MapGet("/{courseId:guid}/instructor", async (
            Guid courseId,
            ISender sender) =>
        {
            var result = await sender.Send(new GetCourseInstructorQuery(courseId));
            return Results.Ok(result);
        }).WithSummary("Get course instructor DoctorId (cross-service ref)");

        // Admin course endpoints
        var admin = app.MapGroup("/api/v1/academic/admin/courses")
                       .RequireAuthorization(p => p.RequireRole("Admin","SuperAdmin", "Doctor"))
                       .WithTags("Academic – Courses (Admin)");

        admin.MapGet("/", async (ISender sender) =>
        {
            var result = await sender.Send(new GetAllCoursesAdminQuery());
            return Results.Ok(result);
        })
        .WithSummary("Get all courses with enrollment count (Admin)");

        admin.MapPost("/", async (
            [FromForm] string name,
            [FromForm] string? description,
            IFormFile? coverImage,
            [FromForm] Guid doctorId,
            [FromForm] Guid? departmentId,
            ISender sender) =>
        {
            var command = new CreateCourseCommand(name, description, coverImage, doctorId, departmentId);
            var result = await sender.Send(command);
            return Results.Created($"/api/v1/academic/courses/{result.Id}", result);
        })
        .DisableAntiforgery()
        .WithSummary("Create a new course (Admin)");

        admin.MapPut("/{courseId:guid}", async (
            Guid courseId,
            [FromForm] string name,
            [FromForm] string? description,
            IFormFile? coverImage,
            [FromForm] Guid doctorId,
            ISender sender) =>
        {
            var result = await sender.Send(new UpdateCourseCommand(
                courseId, name, description, coverImage, doctorId));
            return Results.Ok(result);
        })
        .DisableAntiforgery()
        .WithSummary("Update course (Admin)");

        admin.MapDelete("/{courseId:guid}", async (
            Guid courseId,
            ISender sender) =>
        {
            await sender.Send(new DeleteCourseCommand(courseId));
            return Results.NoContent();
        }).WithSummary("Delete course (Admin)");

        admin.MapPost("/{courseId:guid}/enroll", async (
            Guid courseId,
            [FromBody] EnrollStudentsRequest body,
            ISender sender) =>
        {
            await sender.Send(new EnrollStudentsCommand(courseId, body.StudentIds));
            return Results.Ok(new { message = "Students enrolled successfully." });
        }).WithSummary("Enroll students into a course (Admin)");

        admin.MapGet("/{courseId:guid}/students-details", async (
            Guid courseId,
            ISender sender) =>
        {
            var result = await sender.Send(new GetCourseStudentsDetailsQuery(courseId));
            return Results.Ok(result);
        })
        .WithSummary("Get enrolled students details for a course (Admin/Doctor)");

        admin.MapGet("/{courseId:guid}/lectures", async (
            Guid courseId,
            ISender sender) =>
        {
            var result = await sender.Send(new GetCourseLecturesAdminQuery(courseId));
            return Results.Ok(result);
        })
        .WithSummary("Get all lectures with upload status for a course (Admin)");
    }
}

public record EnrollStudentsRequest(List<Guid> StudentIds);
