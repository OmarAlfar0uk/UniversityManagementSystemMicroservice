using Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Features.Internal;

public static class InternalEndpoints
{
    private static bool IsInternalRequest(HttpContext ctx) =>
        ctx.Request.Headers["X-Internal-Request"] == "true";

    public static void MapInternalEndpoints(this IEndpointRouteBuilder app)
    {
        // GET /api/v1/auth/internal/users/count?role=Student
        app.MapGet("/api/v1/auth/internal/users/count", async (
            string role,
            UserManager<ApplicationUser> userManager,
            HttpContext ctx) =>
        {
            if (!IsInternalRequest(ctx)) return Results.Forbid();

            var usersInRole = await userManager.GetUsersInRoleAsync(role);
            var count = usersInRole.Count(u => u.IsActivated);
            return Results.Ok(count);
        });

        // GET /api/v1/auth/internal/users/{userId}
        app.MapGet("/api/v1/auth/internal/users/{userId:guid}", async (
            Guid userId,
            UserManager<ApplicationUser> userManager,
            HttpContext ctx) =>
        {
            if (!IsInternalRequest(ctx)) return Results.Forbid();

            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null) return Results.NotFound();

            var roles = await userManager.GetRolesAsync(user);
            return Results.Ok(new
            {
                id       = user.Id,
                fullName = user.FullName,
                email    = user.Email,
                role     = roles.FirstOrDefault() ?? string.Empty
            });
        });

        // GET /api/v1/auth/internal/departments/{departmentId}/students
        app.MapGet("/api/v1/auth/internal/departments/{departmentId:guid}/students", async (
            Guid departmentId,
            UserManager<ApplicationUser> userManager,
            HttpContext ctx) =>
        {
            if (!IsInternalRequest(ctx)) return Results.Forbid();

            var students = await userManager.GetUsersInRoleAsync("Student");
            var result = students
                .Where(u => u.DepartmentId == departmentId)
                .Select(u => new
                {
                    id           = u.Id,
                    fullName     = u.FullName,
                    universityId = u.UniversityId,
                    departmentId = u.DepartmentId
                })
                .ToList();

            return Results.Ok(result);
        });
    }
}
