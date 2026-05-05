using AuthService.Features.Auth.Doctor.GetProfile;
using AuthService.Features.Auth.Doctor.UpdateProfile;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Features.Auth.Doctor
{
    public static class DoctorEndpoints
    {
        public static WebApplication MapDoctorAuthEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/v1/auth/doctor")
                           .RequireAuthorization(p => p.RequireRole("Doctor"))
                           .WithTags("Doctor - Auth");

            // GET /api/v1/auth/doctor/profile
            group.MapGet("/profile", async (
                HttpContext context,
                IMediator mediator) =>
            {
                var userId = context.User.FindFirst("id")?.Value
                    ?? throw new InvalidOperationException("User ID claim not found.");
                var query = new GetProfileQuery(Guid.Parse(userId));
                var result = await mediator.Send(query);
                return Results.Ok(result);
            }).WithSummary("Get Doctor profile");

            // PUT /api/v1/auth/doctor/profile
            // PUT /api/v1/auth/doctor/profile
            group.MapPut("/profile", async (
                    HttpContext context,
                    IMediator mediator,
                    [FromForm] string? fullName,
                    [FromForm] string? phoneNumber,
                    IFormFile? profileImage) =>
                {
                    var userId = context.User.FindFirst("id")?.Value
                                 ?? throw new InvalidOperationException("User ID claim not found.");

                    var command = new UpdateProfileCommand(
                        Guid.Parse(userId),
                        string.IsNullOrWhiteSpace(fullName) ? null : fullName,
                        string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber,
                        profileImage
                    );

                    var result = await mediator.Send(command);
                    return Results.Ok(result);
                })
                .DisableAntiforgery()
                .WithSummary("Update Doctor profile");

            return app;
        }
    }
}
