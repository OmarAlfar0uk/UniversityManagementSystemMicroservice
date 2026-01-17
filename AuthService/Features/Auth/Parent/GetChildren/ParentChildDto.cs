namespace AuthService.Features.Auth.Parent.GetChildren
{
    public class ParentChildDto
    {
        public Guid StudentId { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
}
