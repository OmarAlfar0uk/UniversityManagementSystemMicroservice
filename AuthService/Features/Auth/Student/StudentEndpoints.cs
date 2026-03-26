using AuthService.Features.Auth.Student.GetProfile;
using AuthService.Features.Auth.Student.UpdateProfile;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthService.Features.Auth.Student
{
    /// <summary>
    /// Student endpoints — owned by the Student team.
    /// Base route: /api/v1/auth/student
    /// </summary>
    public static class StudentEndpoints
    {
        public static IEndpointRouteBuilder MapStudentAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/auth/student")
                           .WithTags("Student – Auth")
                           .RequireAuthorization(policy => policy.RequireRole("Student"));

            // GET /api/v1/auth/student/profile
            group.MapGet("/profile", async (
                ISender sender,
                ClaimsPrincipal user) =>
            {
                var studentId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await sender.Send(new GetProfileQuery(studentId));
                return Results.Ok(result);
            })
            .WithName("GetStudentProfile")
            .WithSummary("Get the current student's profile");

            // PUT /api/v1/auth/student/profile
            group.MapPut("/profile", async (
                ISender sender,
                ClaimsPrincipal user,
                [FromForm] string? fullName,
                [FromForm] string? phoneNumber,
                IFormFile? profileImage) =>
            {
                var studentId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var command = new UpdateProfileCommand(
                    StudentId:    studentId,
                    FullName:     fullName,
                    PhoneNumber:  phoneNumber,
                    ProfileImage: profileImage
                );
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateStudentProfile")
            .WithSummary("Update the current student's profile (partial update — all fields optional)")
            .DisableAntiforgery();

            return app;
        }
    }
}
