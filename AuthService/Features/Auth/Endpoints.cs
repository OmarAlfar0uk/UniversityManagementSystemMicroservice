using Auth_Service.Features.Shared;
using AuthService.Features.Auth.Activate;
using AuthService.Features.Auth.Admin.ChangeRole;
using AuthService.Features.Auth.Admin.Create;
using AuthService.Features.Auth.Admin.GetUsers;
using AuthService.Features.Auth.Admin.ToggleUserStatus;
using AuthService.Features.Auth.Login;
using AuthService.Features.Auth.Logout;
using AuthService.Features.Auth.Parent.ActivateParent;
using AuthService.Features.Auth.Parent.GenerateCode;
using AuthService.Features.Auth.Parent.GetChildren;
using AuthService.Features.Auth.Refresh;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AuthService.Features.Auth
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/auth")
                           .WithTags("Auth");

            group.MapPost("/activate", Activate)
                 .RequireRateLimiting("activate");
            group.MapPost("/login", Login)
                .RequireRateLimiting("login");
            group.MapPost("/refresh", Refresh);
            group.MapPost("/logout", Logout);
            group.MapPost("/parent/activate", ActivateParent)
                .RequireRateLimiting("activate");
            group.MapPost("/parent/generate-code", GenerateParentCode)
                 .RequireAuthorization()
                 .RequireAuthorization(policy => policy.RequireRole("Student"))
                  .RequireRateLimiting("parent-generate");
            group.MapGet("/parent/children", GetParentChildren)
                 .RequireAuthorization()
                 .RequireAuthorization(policy => policy.RequireRole("Parent"));
            group.MapPost("/admin/create", CreateAdmin)
                 .RequireAuthorization(policy => policy.RequireRole("SuperAdmin"));
            group.MapPut("/admin/change-role", ChangeUserRole)
                 .RequireAuthorization(policy => policy.RequireRole("SuperAdmin"));
            group.MapGet("/admin/users", GetUsers)
                 .RequireAuthorization(policy => policy.RequireRole("Admin", "SuperAdmin"));
            group.MapPut("/admin/users/{userId:guid}/toggle-status", ToggleUserStatus)
                  .RequireAuthorization(policy => policy.RequireRole("Admin", "SuperAdmin"));

            return app;
        }

        public static IResult ToHttpResult<T>(this EndpointResponse<T> response)
            => Results.Json(response, statusCode: response.StatusCode);

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

        private static async Task<IResult> GenerateParentCode(
        GenerateParentCodeCommand command,
        IMediator mediator)
        {
            var result = await mediator.Send(command);
            return result.ToHttpResult();
        }

        private static async Task<IResult> ActivateParent(
        ActivateParentCommand command,
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

        private static async Task<IResult> CreateAdmin(
        CreateAdminCommand command,
        IMediator mediator)
        {
            var result = await mediator.Send(command);
            return result.ToHttpResult();
        }

        private static async Task<IResult> ChangeUserRole(
        ChangeUserRoleCommand command,
        IMediator mediator)
        {
            var result = await mediator.Send(command);
            return result.ToHttpResult();
        }

        private static async Task<IResult> GetUsers(
        [AsParameters] GetUsersQuery query,
        IMediator mediator)
        {
            var result = await mediator.Send(query);
            return result.ToHttpResult();
        }

        private static async Task<IResult> ToggleUserStatus(
        Guid userId,
        bool enable,
        IMediator mediator)
        {
            var command = new ToggleUserStatusCommand
            {
                UserId = userId,
                Enable = enable
            };

            var result = await mediator.Send(command);
            return result.ToHttpResult();
        }

    }
}
