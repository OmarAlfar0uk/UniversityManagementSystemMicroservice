using Auth.Models;

namespace AuthService.Models
{
    public class ParentStudent :BaseEntity
    {
        public Guid ParentId { get; set; }
        public ApplicationUser Parent { get; set; } = default!;
        public Guid StudentId { get; set; }
        public ApplicationUser Student { get; set; } = default!;
    }

}
