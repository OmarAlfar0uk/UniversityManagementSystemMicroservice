using Auth.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Features.Internal;

public static class InternalEndpoints
{
    public static void MapInternalEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth/internal")
                       .RequireAuthorization()
                       .WithTags("Internal \u2013 Auth");

        // GET /api/v1/auth/internal/users/count?role=Student
        group.MapGet("/users/count", async (
            string role,
            UserManager<ApplicationUser> userManager) =>
        {
            var usersInRole = await userManager.GetUsersInRoleAsync(role);
            var count = usersInRole.Count(u => u.IsActivated);
            return Results.Ok(count);
        });

        // GET /api/v1/auth/internal/users/{userId}
        group.MapGet("/users/{userId:guid}", async (
            Guid userId,
            UserManager<ApplicationUser> userManager) =>
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null) return Results.NotFound();

            var roles = await userManager.GetRolesAsync(user);

            // Get department name if DepartmentId exists
            string? department = null;
            if (user.DepartmentId.HasValue)
            {
                // Department lookup would need DbContext - for now return null
                // This can be enhanced later with department service
            }

            return Results.Ok(new
            {
                id              = user.Id,
                firstName       = user.FirstName ?? string.Empty,
                fullName        = user.FullName ?? string.Empty,
                email           = user.Email ?? string.Empty,
                role            = roles.FirstOrDefault() ?? string.Empty,
                profileImageUrl = user.ProfileImageUrl,
                department      = department
            });
        });

        // GET /api/v1/auth/internal/departments/{departmentId}/students
        group.MapGet("/departments/{departmentId:guid}/students", async (
            Guid departmentId,
            UserManager<ApplicationUser> userManager) =>
        {
            var students = await userManager.GetUsersInRoleAsync("Student");
            var result = students
                .Where(u => u.DepartmentId == departmentId)
                .Select(u => new
                {
                    id           = u.Id,
                    firstName    = u.FirstName ?? string.Empty,
                    fullName     = u.FullName ?? string.Empty,
                    universityId = u.UniversityId,
                    departmentId = u.DepartmentId
                })
                .ToList();

            return Results.Ok(result);
        });
    }
}
