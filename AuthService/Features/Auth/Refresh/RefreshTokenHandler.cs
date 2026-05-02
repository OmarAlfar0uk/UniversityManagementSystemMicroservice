using Auth.Contarcts;
using Auth_Service.Features.Shared;
using MediatR;

namespace AuthService.Features.Auth.Refresh
{
    public class RefreshTokenHandler
        : IRequestHandler<RefreshTokenCommand, EndpointResponse<RefreshTokenResponse>>
    {
        private readonly ITokenService _tokenService;

        public RefreshTokenHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task<EndpointResponse<RefreshTokenResponse>> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            var tokenPair =
                await _tokenService.RefreshAccessTokenAsync(request.RefreshToken);

            if (!tokenPair.HasValue)
            {
                return EndpointResponse<RefreshTokenResponse>.UnauthorizedResponse(
                    "Invalid or expired refresh token");
            }

            return EndpointResponse<RefreshTokenResponse>.SuccessResponse(
                new RefreshTokenResponse
                {
                    AccessToken = tokenPair.Value.AccessToken,
                    RefreshToken = tokenPair.Value.RefreshToken
                },
                "Token refreshed successfully"
            );
        }
    }
}
