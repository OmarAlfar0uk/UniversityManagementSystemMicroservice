using Auth.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Models
{
    public class ParentStudent :BaseEntity
    {
        public Guid ParentId { get; set; }
        public ApplicationUser Parent { get; set; } = null!;

        public Guid StudentId { get; set; }
        public ApplicationUser Student { get; set; } = null!;
    }

}
