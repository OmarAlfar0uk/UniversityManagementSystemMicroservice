using Auth.Models;
using AuthService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data
{
    public class UniversitySystemAuthContext
        : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public UniversitySystemAuthContext(DbContextOptions<UniversitySystemAuthContext> options)
            : base(options)
        {
        }
        public DbSet<ActivationCode> ActivationCodes { get; set; }
        public DbSet<ParentCode> ParentCodes { get; set; }
        public DbSet<ParentStudent> ParentStudents { get; set; }
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(UniversitySystemAuthContext).Assembly);

            modelBuilder.Entity<ApplicationUser>().ToTable("Users");
            modelBuilder.Entity<ApplicationRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
        }
    }
}
