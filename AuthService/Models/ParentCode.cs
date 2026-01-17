using Auth.Models;

namespace AuthService.Models
{
    public class ParentCode : BaseEntity
    {
        public string Code { get; set; } = default!;
        public Guid StudentId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; }
    }

}
