using Auth_Service.Features.Shared;
using MediatR;
using Microsoft.AspNetCore.Routing;

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

            // ── TODO: Student team adds their endpoints here ─────────────────────
            //
            // Example:
            // group.MapGet("/profile", GetProfile)
            //      .WithSummary("Get student profile");
            //
            // ─────────────────────────────────────────────────────────────────────

            return app;
        }
    }
}
