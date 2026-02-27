using Auth_Service.Features.Shared;
using AuthService.Features.Auth.Parent.ActivateParent;
using AuthService.Features.Auth.Parent.GenerateCode;
using AuthService.Features.Auth.Parent.GetChildren;
using MediatR;
using Microsoft.AspNetCore.Routing;

namespace AuthService.Features.Auth.Parent
{
    /// <summary>
    /// Parent endpoints — owned by the Parent team.
    /// Base route: /api/v1/auth/parent
    /// </summary>
    public static class ParentEndpoints
    {
        public static IEndpointRouteBuilder MapParentAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/auth/parent")
                           .WithTags("Parent – Auth");

            // POST /api/v1/auth/parent/activate  → Public
            group.MapPost("/activate", ActivateParent)
                 .RequireRateLimiting("activate")
                 .WithSummary("Activate a Parent account")
                 .WithDescription("Activates the parent account using the one-time code sent to their email.");

            // POST /api/v1/auth/parent/generate-code  → Student only
            group.MapPost("/generate-code", GenerateParentCode)
                 .RequireAuthorization()
                 .RequireAuthorization(policy => policy.RequireRole("Student"))
                 .RequireRateLimiting("parent-generate")
                 .WithSummary("Generate a Parent invitation code")
                 .WithDescription("Student generates a one-time code their parent uses to register.");

            // GET /api/v1/auth/parent/children  → Parent only
            group.MapGet("/children", GetParentChildren)
                 .RequireAuthorization()
                 .RequireAuthorization(policy => policy.RequireRole("Parent"))
                 .WithSummary("Get Parent's children")
                 .WithDescription("Returns the list of students linked to the authenticated parent.");

            return app;
        }

        // ── Handlers ────────────────────────────────────────────────────────────

        private static async Task<IResult> ActivateParent(
            ActivateParentCommand command,
            IMediator mediator)
        {
            var result = await mediator.Send(command);
            return result.ToHttpResult();
        }

        private static async Task<IResult> GenerateParentCode(
            GenerateParentCodeCommand command,
            IMediator mediator)
        {
            var result = await mediator.Send(command);
            return result.ToHttpResult();
        }

        private static async Task<IResult> GetParentChildren(
            IMediator mediator)
        {
            var result = await mediator.Send(new GetParentChildrenQuery());
            return result.ToHttpResult();
        }
    }
}
