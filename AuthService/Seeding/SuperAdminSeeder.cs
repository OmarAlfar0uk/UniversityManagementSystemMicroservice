using Auth.Models;
using AuthService.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Seeding
{
    public static class SuperAdminSeeder
    {
        public static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager)
        {   
            var superAdminUserName = "SuperAdmin";

            var existingUser = await userManager.FindByNameAsync(superAdminUserName);
            if (existingUser != null) return;

            var user = new ApplicationUser
            {
                UserName = superAdminUserName,
                FirstName = "Omar",
                LastName = "Alfarouk",
                IsActivated = true,
                Gender = Gender.Male
            };

            var result = await userManager.CreateAsync(user, "SuperAdmin@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "SuperAdmin");
            }
        }
    }
}
