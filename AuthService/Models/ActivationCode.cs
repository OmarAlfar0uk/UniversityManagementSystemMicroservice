using Auth.Models;

namespace AuthService.Models
{
    public class ActivationCode : BaseEntity
    {
        public string Code { get; set; } = default!;
        public Guid UserId { get; set; }
        public string Role { get; set; } = default!;
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; }
    }

}
