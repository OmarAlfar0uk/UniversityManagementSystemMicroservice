using Auth.Models;
using Auth_Service.Features.Shared;
using AuthService.Contracts;
using AuthService.Data;
using AuthService.Features.Auth.Admin.CreateStudent;
using AuthService.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AuthService.Features.Auth.Admin.CreateDoctor
{
    public class CreateDoctorHandler
        : IRequestHandler<CreateDoctorCommand, EndpointResponse<CreateDoctorResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UniversitySystemAuthContext _context;
        private readonly IAuditLogger _auditLogger;

        public CreateDoctorHandler(
            UserManager<ApplicationUser> userManager,
            UniversitySystemAuthContext context,
            IAuditLogger auditLogger)
        {
            _userManager = userManager;
            _context = context;
            _auditLogger = auditLogger;
        }

        public async Task<EndpointResponse<CreateDoctorResponse>> Handle(
            CreateDoctorCommand request,
            CancellationToken cancellationToken)
        {
            // 1️⃣ Email uniqueness
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                return EndpointResponse<CreateDoctorResponse>
                    .ErrorResponse("Email already exists", 409);
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                if (!Enum.TryParse<Gender>(request.Gender, true, out var gender))
                {
                    return EndpointResponse<CreateDoctorResponse>.ErrorResponse("Invalid gender value", 400);

                }

                // 2️⃣ Create doctor user
                var doctor = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = request.Email,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Gender = gender,
                    IsActivated = false,
                    EmailConfirmed = false
                };

                var createResult = await _userManager.CreateAsync(doctor);
                if (!createResult.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken);

                    return EndpointResponse<CreateDoctorResponse>.ErrorResponse(
                        "Failed to create doctor",
                        400,
                        createResult.Errors.Select(e => e.Description).ToList()
                    );
                }

                // 3️⃣ Assign Doctor role
                var roleResult = await _userManager.AddToRoleAsync(doctor, "Doctor");
                if (!roleResult.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken);

                    return EndpointResponse<CreateDoctorResponse>.ErrorResponse(
                        "Failed to assign doctor role",
                        400,
                        roleResult.Errors.Select(e => e.Description).ToList()
                    );
                }

                // 4️⃣ Generate activation code
                var code = Guid.NewGuid().ToString("N")[..6].ToUpper();

                var activationCode = new ActivationCode
                {
                    Id = Guid.NewGuid(),
                    Code = code,
                    UserId = doctor.Id,
                    Role = "Doctor",
                    ExpiryDate = DateTime.UtcNow.AddDays(3),
                    IsUsed = false
                };

                _context.ActivationCodes.Add(activationCode);
                await _context.SaveChangesAsync(cancellationToken);

                // 5️⃣ Commit
                await transaction.CommitAsync(cancellationToken);

                // 6️⃣ Audit (outside transaction)
                await _auditLogger.LogAsync(
                    action: "CreateDoctor",
                    targetId: doctor.Id.ToString(),
                    description: $"Doctor created with email {doctor.Email}"
                );

                return EndpointResponse<CreateDoctorResponse>.SuccessResponse(
                    new CreateDoctorResponse
                    {
                        DoctorId = doctor.Id,
                        Email = doctor.Email!,
                        ActivationCode = code
                    },
                    "Doctor created successfully",
                    201
                );
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                Log.Error(ex, "CreateDoctor failed for {Email}", request.Email);

                return EndpointResponse<CreateDoctorResponse>.ErrorResponse(
                    "Unexpected error occurred while creating doctor",
                    500
                );
            }
        }
    }
}
