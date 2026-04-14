using Auth.Models;
using AuthService.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Seeding
{
    public static class SuperAdminSeeder
    {
        public static async Task SeedSuperAdminAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            const string roleName = "SuperAdmin";
            const string email = "superadmin@university.com";   
            const string password = "SuperAdmin@123";

            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new ApplicationRole
                {
                    Name = roleName,
                    Description = "System Owner"
                });
            }

            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser != null)
                return;

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = "Omar",
                LastName = "Alfarouk",
                Gender = Gender.Male,
                IsActivated = true,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                throw new Exception(
                    $"Failed to create SuperAdmin: {string.Join(", ", result.Errors.Select(e => e.Description))}"
                );
            }

            await userManager.AddToRoleAsync(user, roleName);
        }
    }
}
