namespace AuthService.Features.Auth.Parent.GenerateCode
{
    public class GenerateParentCodeResponse
    {
        public string Code { get; set; } = default!;
        public DateTime ExpiryDate { get; set; }
    }
}
