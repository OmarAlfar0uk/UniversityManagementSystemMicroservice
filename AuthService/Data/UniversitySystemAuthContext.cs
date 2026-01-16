using Auth.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AuthService.Data
{
    public class UniversitySystemAuthContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
   

        public UniversitySystemAuthContext(DbContextOptions<UniversitySystemAuthContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ✅ Always call base first to configure Identity properly
            base.OnModelCreating(modelBuilder);

            // ✅ Apply all IEntityTypeConfiguration from current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UniversitySystemAuthContext).Assembly);

            // ✅ Optional: Customize Identity table names if you want clean names
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("Users");
            });

            modelBuilder.Entity<IdentityRole<Guid>>(entity =>
            {
                entity.ToTable("Roles");
            });

            modelBuilder.Entity<IdentityUserRole<Guid>>(entity =>
            {
                entity.ToTable("UserRoles");
            });

            modelBuilder.Entity<IdentityUserClaim<Guid>>(entity =>
            {
                entity.ToTable("UserClaims");
            });

            modelBuilder.Entity<IdentityUserLogin<Guid>>(entity =>
            {
                entity.ToTable("UserLogins");
            });

            modelBuilder.Entity<IdentityRoleClaim<Guid>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });

            modelBuilder.Entity<IdentityUserToken<Guid>>(entity =>
            {
                entity.ToTable("UserTokens");
            });
        }
    }
}
