using System.Security.Claims;
using AttendanceService.Features.Attendance.GenerateAttendanceCode;
using AttendanceService.Features.Attendance.GetAttendancePercentage;
using AttendanceService.Features.Attendance.GetAttendanceStatus;
using AttendanceService.Features.Attendance.GetChildAttendance;
using AttendanceService.Features.Attendance.GetCoursesAttendance;
using AttendanceService.Features.Attendance.GetLectureAttendees;
using AttendanceService.Features.Attendance.GetLecturesAttendance;
using AttendanceService.Features.Attendance.ManualAttendance;
using AttendanceService.Features.Attendance.RegisterAttendance;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceService.Features.Attendance;

public static class Endpoints
{
    public static void MapAttendanceEndpoints(this IEndpointRouteBuilder app)
    {
        // Student endpoints
        var student = app.MapGroup("/api/v1/attendance")
                         .RequireAuthorization()
                         .WithTags("Attendance – Student");

        student.MapGet("/courses", async (ISender sender, ClaimsPrincipal user) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await sender.Send(new GetCoursesAttendanceQuery(studentId));
            return Results.Ok(result);
        }).WithSummary("Get courses attendance (Student)");

        student.MapGet("/courses/{courseId:guid}/lectures", async (
            Guid courseId, ISender sender, ClaimsPrincipal user) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await sender.Send(new GetLecturesAttendanceQuery(courseId, studentId));
            return Results.Ok(result);
        }).WithSummary("Get lectures attendance for a course (Student)");

        student.MapGet("/courses/{courseId:guid}/percentage", async (
            Guid courseId, ISender sender, ClaimsPrincipal user) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await sender.Send(new GetAttendancePercentageQuery(courseId, studentId));
            return Results.Ok(result);
        }).WithSummary("Get attendance percentage for a course (Student)");

        student.MapGet("/lectures/{lectureId:guid}/status", async (
            Guid lectureId, ISender sender, ClaimsPrincipal user) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await sender.Send(new GetAttendanceStatusQuery(lectureId, studentId));
            return Results.Ok(result);
        }).WithSummary("Get attendance status for a lecture (Student)");

        student.MapPost("/lectures/{lectureId:guid}/register", async (
            Guid lectureId,
            [FromBody] RegisterAttendanceBody body,
            ISender sender,
            ClaimsPrincipal user) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await sender.Send(new RegisterAttendanceCommand(lectureId, studentId, body.Code));
            return Results.Ok(new { message = "Attendance Has Been Registered Successfully" });
        }).WithSummary("Register attendance with code (Student)");

        // Doctor endpoints
        var doctor = app.MapGroup("/api/v1/attendance/doctor")
                        .RequireAuthorization()
                        .WithTags("Attendance – Doctor");

        doctor.MapPost("/lectures/{lectureId:guid}/generate-code", async (
            Guid lectureId,
            [FromBody] GenerateCodeBody body,
            ISender sender) =>
        {
            var result = await sender.Send(new GenerateAttendanceCodeCommand(lectureId, body.CourseId, body.ExpiresInMinutes));
            return Results.Ok(result);
        }).WithSummary("Generate attendance code (Doctor)");

        doctor.MapGet("/lectures/{lectureId:guid}/students", async (
            Guid lectureId, ISender sender) =>
        {
            var result = await sender.Send(new GetLectureAttendeesQuery(lectureId));
            return Results.Ok(result);
        }).WithSummary("Get lecture attendees (Doctor)");

        doctor.MapPut("/lectures/{lectureId:guid}/manual", async (
            Guid lectureId,
            [FromBody] ManualAttendanceBody body,
            ISender sender) =>
        {
            await sender.Send(new ManualAttendanceCommand(lectureId, body.StudentId, body.IsAttended));
            return Results.Ok(new { message = "Attendance updated." });
        }).WithSummary("Manual attendance override (Doctor)");

        // Parent endpoints
        var parent = app.MapGroup("/api/v1/attendance/parent")
                        .RequireAuthorization()
                        .WithTags("Attendance – Parent");

        parent.MapGet("/children/{studentId:guid}/courses", async (
            Guid studentId, ISender sender) =>
        {
            var result = await sender.Send(new GetChildAttendanceQuery(studentId));
            return Results.Ok(result);
        }).WithSummary("Get child attendance (Parent)");
    }
}

public record RegisterAttendanceBody(string Code);
public record GenerateCodeBody(Guid CourseId, int ExpiresInMinutes);
public record ManualAttendanceBody(Guid StudentId, bool IsAttended);
