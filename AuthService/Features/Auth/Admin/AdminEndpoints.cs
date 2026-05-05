using Auth_Service.Features.Shared;
using AuthService.Features.Auth.Admin.ChangeRole;
using AuthService.Features.Auth.Admin.Create;
using AuthService.Features.Auth.Admin.CreateDoctor;
using AuthService.Features.Auth.Admin.CreateStudent;
using AuthService.Features.Auth.Admin.DeleteUser;
using AuthService.Features.Auth.Admin.GetProfile;
using AuthService.Features.Auth.Admin.GetStudentsByDepartment;
using AuthService.Features.Auth.Admin.GetUsers;
using AuthService.Features.Auth.Admin.ToggleUserStatus;
using AuthService.Features.Auth.Admin.UpdateProfile;
using AuthService.Features.Auth.Admin.UpdateUserEmail;
using MediatR;
using Microsoft.AspNetCore.Routing;

namespace AuthService.Features.Auth.Admin
{
    /// <summary>
    /// Admin endpoints — owned by the Admin team.
    /// Base route: /api/v1/auth/admin
    /// Required roles: Admin | SuperAdmin  (SuperAdmin-only where noted)
    /// </summary>
    public static class AdminEndpoints
    {
        public static IEndpointRouteBuilder MapAdminAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/auth/admin")
                           .WithTags("Admin – Auth");

            // POST /api/v1/auth/admin/create  → SuperAdmin only
            group.MapPost("/create", CreateAdmin)
                 .RequireAuthorization(policy => policy.RequireRole("SuperAdmin"))
                 .WithSummary("Create a new Admin account")
                 .WithDescription("SuperAdmin only. Creates an admin user and sends activation email.");

            // PUT /api/v1/auth/admin/change-role  → SuperAdmin only
            group.MapPut("/change-role", ChangeUserRole)
                 .RequireAuthorization(policy => policy.RequireRole("SuperAdmin"))
                 .WithSummary("Change a user's role")
                 .WithDescription("SuperAdmin only. Changes the role assigned to an existing user.");

            // GET /api/v1/auth/admin/users
            group.MapGet("/users", GetUsers)
                 .RequireAuthorization(policy => policy.RequireRole("Admin", "SuperAdmin"))
                 .WithSummary("List all users")
                 .WithDescription("Returns a paginated list of users. Accessible by Admin and SuperAdmin.");

            group.MapGet("/departments/{departmentId:guid}/students", GetStudentsByDepartment)
                 .RequireAuthorization(policy => policy.RequireRole("Admin", "SuperAdmin"))
                 .WithTags("Admin – Students")
                 .WithSummary("List students by department")
                 .WithDescription("Returns all student accounts assigned to the given department.");

            // PUT /api/v1/auth/admin/users/{userId}/toggle-status
            group.MapPut("/users/{userId:guid}/toggle-status", ToggleUserStatus)
                 .RequireAuthorization(policy => policy.RequireRole("Admin", "SuperAdmin"))
                 .WithSummary("Enable or disable a user account")
                 .WithDescription("Toggles the active/inactive status of a user. Accessible by Admin and SuperAdmin.");

            // POST /api/v1/auth/admin/students
            group.MapPost("/students", CreateStudent)
                 .RequireAuthorization(policy => policy.RequireRole("Admin", "SuperAdmin"))
                 .WithTags("Admin – Students")
                 .WithSummary("Create a new Student account")
                 .WithDescription("Creates a student user and sends activation email.");

            // POST /api/v1/auth/admin/doctors
            group.MapPost("/doctors", CreateDoctor)
                 .RequireAuthorization(policy => policy.RequireRole("Admin", "SuperAdmin"))
                 .WithTags("Admin – Doctors")
                 .WithSummary("Create a new Doctor account")
                 .WithDescription("Creates a doctor user and sends activation email.");

            // DELETE /api/v1/auth/admin/users/{userId}
            group.MapDelete("/users/{userId:guid}", DeleteUser)
                 .RequireAuthorization(policy => policy.RequireRole("Admin", "SuperAdmin"))
                 .WithSummary("Delete a user")
                 .WithDescription("Permanently deletes a user and all their Identity data (roles, claims, tokens). Cannot delete SuperAdmin.");

            // PUT /api/v1/auth/admin/users/{userId}/email
            group.MapPut("/users/{userId:guid}/email", UpdateUserEmail)
                 .RequireAuthorization(policy => policy.RequireRole("Admin", "SuperAdmin"))
                 .WithSummary("Update a user's email")
                 .WithDescription("Updates the user email correctly (NormalizedEmail included). Use this instead of editing the DB manually.");

            // GET /api/v1/auth/admin/profile
            group.MapGet("/profile", GetAdminProfile)
                 .RequireAuthorization(policy => policy.RequireRole("Admin", "SuperAdmin"))
                 .WithSummary("Get Admin profile")
                 .WithDescription("Returns the profile of the currently authenticated Admin or SuperAdmin.");

            // PUT /api/v1/auth/admin/profile
            group.MapPut("/profile", UpdateAdminProfile)
                 .RequireAuthorization(policy => policy.RequireRole("Admin", "SuperAdmin"))
                 .WithSummary("Update Admin profile")
                 .WithDescription("Updates the profile of the currently authenticated Admin or SuperAdmin. Supports multipart/form-data for image upload.");

            return app;
        }

        // ── Handlers ────────────────────────────────────────────────────────────

        private static async Task<IResult> CreateAdmin(
            CreateAdminCommand command,
            IMediator mediator)
        {
            var result = await mediator.Send(command);
            return result.ToHttpResult();
        }

        private static async Task<IResult> ChangeUserRole(
            ChangeUserRoleCommand command,
            IMediator mediator)
        {
            var result = await mediator.Send(command);
            return result.ToHttpResult();
        }

        private static async Task<IResult> GetUsers(
            [AsParameters] GetUsersQuery query,
            IMediator mediator)
        {
            var result = await mediator.Send(query);
            return result.ToHttpResult();
        }

        private static async Task<IResult> GetStudentsByDepartment(
            Guid departmentId,
            IMediator mediator)
        {
            var result = await mediator.Send(new GetStudentsByDepartmentQuery(departmentId));
            return result.ToHttpResult();
        }

        private static async Task<IResult> ToggleUserStatus(
            Guid userId,
            bool enable,
            IMediator mediator)
        {
            var command = new ToggleUserStatusCommand
            {
                UserId = userId,
                Enable = enable
            };
            var result = await mediator.Send(command);
            return result.ToHttpResult();
        }

        private static async Task<IResult> CreateStudent(
            CreateStudentCommand command,
            IMediator mediator)
        {
            var result = await mediator.Send(command);
            return result.ToHttpResult();
        }

        private static async Task<IResult> CreateDoctor(
            CreateDoctorCommand command,
            IMediator mediator)
        {
            var result = await mediator.Send(command);
            return result.ToHttpResult();
        }

        private static async Task<IResult> DeleteUser(
            Guid userId,
            IMediator mediator)
        {
            var result = await mediator.Send(new DeleteUserCommand { UserId = userId });
            return result.ToHttpResult();
        }

        private static async Task<IResult> UpdateUserEmail(
            Guid userId,
            UpdateUserEmailCommand command,
            IMediator mediator)
        {
            command.UserId = userId;
            var result = await mediator.Send(command);
            return result.ToHttpResult();
        }

        private static async Task<IResult> GetAdminProfile(
            HttpContext context,
            IMediator mediator)
        {
            var userId = context.User.FindFirst("id")?.Value
                ?? throw new InvalidOperationException("User ID claim not found.");
            var query = new GetProfileQuery(Guid.Parse(userId));
            var result = await mediator.Send(query);
            return Results.Ok(result);
        }

        private static async Task<IResult> UpdateAdminProfile(
            HttpContext context,
            IMediator mediator,
            HttpRequest request)
        {
            var userId = context.User.FindFirst("id")?.Value
                ?? throw new InvalidOperationException("User ID claim not found.");

            var fullName = request.Form["fullName"].FirstOrDefault();
            var phoneNumber = request.Form["phoneNumber"].FirstOrDefault();
            var profileImage = request.Form.Files["profileImage"];

            var command = new UpdateProfileCommand(
                Guid.Parse(userId),
                string.IsNullOrWhiteSpace(fullName) ? null : fullName,
                string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber,
                profileImage
            );

            var result = await mediator.Send(command);
            return Results.Ok(result);
        }
    }
}
