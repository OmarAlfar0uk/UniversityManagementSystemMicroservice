namespace AuthService.Features.Auth.Parent.ActivateParent
{
    public class ActivateParentResponse
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public string Role { get; set; } = "Parent";
    }
}
