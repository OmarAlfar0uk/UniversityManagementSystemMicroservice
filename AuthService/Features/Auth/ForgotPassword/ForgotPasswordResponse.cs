namespace AuthService.Features.Auth.ForgotPassword
{
    public class ForgotPasswordResponse
    {
        public string MaskedEmail { get; set; } = default!;
        public int RetryAfterSeconds { get; set; }
        public string Message { get; set; } = default!;
    }
}
