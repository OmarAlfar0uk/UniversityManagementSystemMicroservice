using Auth.Models;
using Auth_Service.Features.Shared;
using AuthService.Contracts;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Features.Auth.Admin.Create
{
    public class CreateAdminHandler : IRequestHandler<CreateAdminCommand, EndpointResponse<CreateAdminResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuditLogger _auditLogger;
        public CreateAdminHandler(UserManager<ApplicationUser> userManager , IAuditLogger auditLogger)
        {
            _userManager = userManager;
            _auditLogger = auditLogger;
        }

        public async Task<EndpointResponse<CreateAdminResponse>> Handle(
            CreateAdminCommand request,
            CancellationToken cancellationToken)
        {
            // 1️⃣ Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return EndpointResponse<CreateAdminResponse>.ErrorResponse(
                    "Email already exists",
                    409
                );
            }

            // 2️⃣ Create admin user
            var adminUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsActivated = true,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(adminUser, request.Password);
            if (!result.Succeeded)
            {
                return EndpointResponse<CreateAdminResponse>.ErrorResponse(
                    "Failed to create admin",
                    400,
                    result.Errors.Select(e => e.Description).ToList()
                );
            }

            // 3️⃣ Assign Admin role
            await _userManager.AddToRoleAsync(adminUser, "Admin");

            await _auditLogger.LogAsync(
            action: "CreateAdmin",
            targetId: adminUser.Id.ToString(),
            description: $"Admin created with email {adminUser.Email}"
              );          

            return EndpointResponse<CreateAdminResponse>.SuccessResponse(
                new CreateAdminResponse
                {
                    AdminId = adminUser.Id,
                    Email = adminUser.Email!,
                    Role = "Admin"
                },
                "Admin created successfully",
                201
            );
       

        }
    }
}
