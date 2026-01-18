using Auth.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Models
{
    public class ParentStudent :BaseEntity
    {
        public Guid ParentId { get; set; }

        [ForeignKey(nameof(ParentId))]
        public ApplicationUser Parent { get; set; } = default!;
        public Guid StudentId { get; set; }

        [ForeignKey(nameof(StudentId))]
        public ApplicationUser Student { get; set; } = default!;
    }

}
