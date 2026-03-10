namespace Auth_Service.Features.Shared
{
    public static class EndpointResponseExtensions
    {
        public static IResult ToHttpResult<T>(this EndpointResponse<T> response)
            => Results.Json(response, statusCode: response.StatusCode);
    }
}
