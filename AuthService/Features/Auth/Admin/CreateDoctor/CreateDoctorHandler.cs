using Auth.Contarcts;
using Auth.Models;
using Auth_Service.Features.Shared;
using AuthService.Contracts;
using AuthService.Data;
using AuthService.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace AuthService.Features.Auth.Admin.CreateDoctor
{
    public class CreateDoctorHandler
        : IRequestHandler<CreateDoctorCommand, EndpointResponse<CreateDoctorResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UniversitySystemAuthContext _context;
        private readonly IAuditLogger _auditLogger;
        private readonly IMailKitEmailService _emailService;

        public CreateDoctorHandler(
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

        public async Task<EndpointResponse<CreateDoctorResponse>> Handle(
            CreateDoctorCommand request,
            CancellationToken cancellationToken)
        {
            // 1️⃣ Email uniqueness
            if (await _userManager.FindByEmailAsync(request.Email) != null)
                return EndpointResponse<CreateDoctorResponse>.ErrorResponse("Email already exists", 409);

            if (!Enum.TryParse<Gender>(request.Gender, true, out var gender))
                return EndpointResponse<CreateDoctorResponse>.ErrorResponse("Invalid gender value", 400);

            // ── DB Transaction scope ─────────────────────────────────────────────
            ApplicationUser doctor;
            string code;

            await using var transaction =
                await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                doctor = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = request.Email,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Gender = gender,
                    IsActivated = false,
                    EmailConfirmed = false,
                    UniversityId = "DOC-" + Guid.NewGuid().ToString("N")[..10]
                };

                var createResult = await _userManager.CreateAsync(doctor);
                if (!createResult.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return EndpointResponse<CreateDoctorResponse>.ErrorResponse(
                        "Failed to create doctor", 400,
                        createResult.Errors.Select(e => e.Description).ToList());
                }

                var roleResult = await _userManager.AddToRoleAsync(doctor, "Doctor");
                if (!roleResult.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return EndpointResponse<CreateDoctorResponse>.ErrorResponse(
                        "Failed to assign doctor role", 400,
                        roleResult.Errors.Select(e => e.Description).ToList());
                }

                code = Guid.NewGuid().ToString("N")[..6].ToUpper();

                _context.ActivationCodes.Add(new ActivationCode
                {
                    Id = Guid.NewGuid(),
                    Code = code,
                    UserId = doctor.Id,
                    Role = "Doctor",
                    ExpiryDate = DateTime.UtcNow.AddDays(3),
                    IsUsed = false
                });

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                // ✅ Doctor is now safely in the database
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                Log.Error(ex, "CreateDoctor DB transaction failed for {Email}", request.Email);
                return EndpointResponse<CreateDoctorResponse>.ErrorResponse(
                    "Failed to create doctor due to a database error", 500);
            }

            // ── Post-commit side effects ─────────────────────────────────────────
            try
            {
                await _emailService.SendActivationEmailAsync(
                    doctor.Email!,
                    $"{doctor.FirstName} {doctor.LastName}",
                    code,
                    "Doctor",
                    doctor.UniversityId);
            }
            catch (Exception emailEx)
            {
                Log.Warning(emailEx, "Activation email failed for doctor {Email} (doctor still created)", doctor.Email);
            }

            await _auditLogger.LogAsync(
                action: "CreateDoctor",
                targetId: doctor.Id.ToString(),
                description: $"Doctor created with email {doctor.Email}");

            return EndpointResponse<CreateDoctorResponse>.SuccessResponse(
                new CreateDoctorResponse
                {
                    DoctorId = doctor.Id,
                    Email = doctor.Email!,
                    ActivationCode = code
                },
                "Doctor created successfully",
                201);
        }
    }
}
