using Auth_Service.Features.Shared;
using AuthService.Features.Auth.Activate;
using AuthService.Features.Auth.Login;
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

            group.MapPost("/activate", Activate);
            group.MapPost("/login", Login);

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
    }
}
