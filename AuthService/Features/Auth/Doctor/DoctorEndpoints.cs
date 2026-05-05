using Auth_Service.Features.Shared;
using AuthService.Features.Auth.Doctor.GetProfile;
using AuthService.Features.Auth.Doctor.UpdateProfile;
using MediatR;
using Microsoft.AspNetCore.Routing;

namespace AuthService.Features.Auth.Doctor
{
    /// <summary>
    /// Doctor endpoints — owned by the Doctor team.
    /// Base route: /api/v1/auth/doctor
    /// Required roles: Doctor
    /// </summary>
    public static class DoctorEndpoints
    {
        public static IEndpointRouteBuilder MapDoctorAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/auth/doctor")
                           .RequireAuthorization(policy => policy.RequireRole("Doctor"))
                           .WithTags("Doctor – Auth");

            // GET /api/v1/auth/doctor/profile
            group.MapGet("/profile", GetDoctorProfile)
                 .WithSummary("Get Doctor profile")
                 .WithDescription("Returns the profile of the currently authenticated Doctor.");

            // PUT /api/v1/auth/doctor/profile
            group.MapPut("/profile", UpdateDoctorProfile)
                 .WithSummary("Update Doctor profile")
                 .WithDescription("Updates the profile of the currently authenticated Doctor. Supports multipart/form-data for image upload.");

            return app;
        }

        private static async Task<IResult> GetDoctorProfile(
            HttpContext context,
            IMediator mediator)
        {
            var userId = context.User.FindFirst("id")?.Value
                ?? throw new InvalidOperationException("User ID claim not found.");
            var query = new GetProfileQuery(Guid.Parse(userId));
            var result = await mediator.Send(query);
            return Results.Ok(result);
        }

        private static async Task<IResult> UpdateDoctorProfile(
            HttpContext context,
            IMediator mediator,
            HttpRequest request)
        {
            var userId = context.User.FindFirst("id")?.Value
                ?? throw new InvalidOperationException("User ID claim not found.");

            var fullName = request.Form["fullName"].FirstOrDefault();
            var phoneNumber = request.Form["phoneNumber"].FirstOrDefault();
            var profileImage = request.Form.Files["profileImage"];

            var command = new UpdateProfileCommand(
                Guid.Parse(userId),
                string.IsNullOrWhiteSpace(fullName) ? null : fullName,
                string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber,
                profileImage
            );

            var result = await mediator.Send(command);
            return Results.Ok(result);
        }
    }
}
