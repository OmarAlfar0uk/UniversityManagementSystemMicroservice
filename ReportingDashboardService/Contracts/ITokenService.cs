namespace ReportingDashboardService.Contracts
{
    public interface ITokenService
    {
        Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(
            string userId,
            string email,
            IEnumerable<string> roles,
            bool rememberMe);

        Task<string?> RefreshAccessTokenAsync(string refreshToken);

        Task RevokeRefreshTokenAsync(string userId);
    }
}
