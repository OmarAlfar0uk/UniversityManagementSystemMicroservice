using Auth.Contarcts;
using Auth.Models;
using Auth_Service.Features.Shared;
using AuthService.Contracts;
using AuthService.Data;
using AuthService.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace AuthService.Features.Auth.Admin.Create
{
    public class CreateAdminHandler
        : IRequestHandler<CreateAdminCommand, EndpointResponse<CreateAdminResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UniversitySystemAuthContext _context;
        private readonly IAuditLogger _auditLogger;
        private readonly IMailKitEmailService _emailService;

        public CreateAdminHandler(
            UserManager<ApplicationUser> userManager,
            UniversitySystemAuthContext context,
            IAuditLogger auditLogger,
            IMailKitEmailService emailService)
        {
            _userManager = userManager;
            _context = context;
            _auditLogger = auditLogger;
            _emailService = emailService;
        }

        public async Task<EndpointResponse<CreateAdminResponse>> Handle(
            CreateAdminCommand request,
            CancellationToken cancellationToken)
        {
            // 1️⃣ Check if email exists
            if (await _userManager.FindByEmailAsync(request.Email) != null)
                return EndpointResponse<CreateAdminResponse>.ErrorResponse("Email already exists", 409);

            // ── DB Transaction scope ─────────────────────────────────────────────
            ApplicationUser adminUser;
            string activationCode;

            await using var transaction =
                await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                adminUser = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = request.Email,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    IsActivated = false,
                    EmailConfirmed = false
                };

                var result = await _userManager.CreateAsync(adminUser, request.Password);
                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return EndpointResponse<CreateAdminResponse>.ErrorResponse(
                        "Failed to create admin", 400,
                        result.Errors.Select(e => e.Description).ToList());
                }

                var roleResult = await _userManager.AddToRoleAsync(adminUser, "Admin");
                if (!roleResult.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return EndpointResponse<CreateAdminResponse>.ErrorResponse(
                        "Failed to assign role", 400,
                        roleResult.Errors.Select(e => e.Description).ToList());
                }

                // Admin gets an activation code just like Student/Doctor
                activationCode = Guid.NewGuid().ToString("N")[..6].ToUpper();

                _context.ActivationCodes.Add(new ActivationCode
                {
                    Id = Guid.NewGuid(),
                    Code = activationCode,
                    UserId = adminUser.Id,
                    Role = "Admin",
                    ExpiryDate = DateTime.UtcNow.AddDays(3),
                    IsUsed = false
                });

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                // ✅ Admin is now safely in the database
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                Log.Error(ex, "CreateAdmin DB transaction failed for {Email}", request.Email);
                return EndpointResponse<CreateAdminResponse>.ErrorResponse(
                    "Failed to create admin due to a database error", 500);
            }

            // ── Post-commit: send activation email ──────────────────────────────
            try
            {
                await _emailService.SendActivationEmailAsync(
                    adminUser.Email!,
                    $"{adminUser.FirstName} {adminUser.LastName}",
                    activationCode,
                    "Admin");
            }
            catch (Exception emailEx)
            {
                Log.Warning(emailEx, "Activation email failed for admin {Email} (admin still created)", adminUser.Email);
            }

            await _auditLogger.LogAsync(
                action: "CreateAdmin",
                targetId: adminUser.Id.ToString(),
                description: $"Admin created with email {adminUser.Email}");

            return EndpointResponse<CreateAdminResponse>.SuccessResponse(
                new CreateAdminResponse
                {
                    AdminId = adminUser.Id,
                    Email = adminUser.Email!,
                    Role = "Admin"
                },
                "Admin created successfully",
                201);
        }
    }
}
