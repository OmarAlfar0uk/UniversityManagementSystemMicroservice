using Auth_Service.Features.Shared;
using AuthService.Features.Auth.Activate;
using AuthService.Features.Auth.Login;
using AuthService.Features.Auth.Logout;
using AuthService.Features.Auth.Refresh;
using MediatR;
using Microsoft.AspNetCore.Routing;

namespace AuthService.Features.Auth
{
    /// <summary>
    /// Shared / public Auth endpoints (no role required).
    /// Base route: /api/v1/auth
    ///
    /// Role-specific endpoints live in their own files:
    ///   → Admin:   Features/Auth/Admin/AdminEndpoints.cs   (MapAdminAuthEndpoints)
    ///   → Parent:  Features/Auth/Parent/ParentEndpoints.cs (MapParentAuthEndpoints)
    ///   → Student: Features/Auth/Student/StudentEndpoints.cs (MapStudentAuthEndpoints)
    /// </summary>
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/auth")
                           .WithTags("Auth – Shared");

            // POST /api/v1/auth/activate
            group.MapPost("/activate", Activate)
                 .RequireRateLimiting("activate")
                 .WithSummary("Activate a user account")
                 .WithDescription("Activates a user account using the one-time code received by email.");

            // POST /api/v1/auth/login
            group.MapPost("/login", Login)
                 .RequireRateLimiting("login")
                 .WithSummary("Login")
                 .WithDescription("Authenticates a user and returns JWT access + refresh tokens.");

            // POST /api/v1/auth/refresh
            group.MapPost("/refresh", Refresh)
                 .WithSummary("Refresh access token")
                 .WithDescription("Issues a new access token using a valid refresh token.");

            // POST /api/v1/auth/logout
            group.MapPost("/logout", Logout)
                 .WithSummary("Logout")
                 .WithDescription("Revokes the current refresh token.");

            return app;
        }

        // ── Handlers (ToHttpResult is in Auth_Service.Features.Shared) ──────────

        private static async Task<IResult> Activate(
            ActivateCommand command,
            IMediator mediator)
        {
            var result = await mediator.Send(command);
            return result.ToHttpResult();
        }

        private static async Task<IResult> Login(
            LoginCommand command,
            IMediator mediator)
        {
            var result = await mediator.Send(command);
            return result.ToHttpResult();
        }

        private static async Task<IResult> Refresh(
            RefreshTokenCommand command,
            IMediator mediator)
        {
            var result = await mediator.Send(command);
            return result.ToHttpResult();
        }

        private static async Task<IResult> Logout(
            LogoutCommand command,
            IMediator mediator)
        {
            var result = await mediator.Send(command);
            return result.ToHttpResult();
        }
    }
}
