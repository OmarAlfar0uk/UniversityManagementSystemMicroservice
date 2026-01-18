using Auth.Models;
using Auth_Service.Features.Shared;
using AuthService.Contracts;
using AuthService.Data;
using AuthService.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AuthService.Features.Auth.Admin.CreateStudent
{
    public class CreateStudentHandler
        : IRequestHandler<CreateStudentCommand, EndpointResponse<CreateStudentResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UniversitySystemAuthContext _context;
        private readonly IAuditLogger _auditLogger;

        public CreateStudentHandler(
            UserManager<ApplicationUser> userManager,
            UniversitySystemAuthContext context,
            IAuditLogger auditLogger)
        {
            _userManager = userManager;
            _context = context;
            _auditLogger = auditLogger;
        }

        public async Task<EndpointResponse<CreateStudentResponse>> Handle(
            CreateStudentCommand request,
            CancellationToken cancellationToken)
        {
            // 1️⃣ Check email uniqueness
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                return EndpointResponse<CreateStudentResponse>
                    .ErrorResponse("Email already exists", 409);
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
              if (!Enum.TryParse<Gender>(request.Gender, true, out var gender))
                    {
                    return EndpointResponse<CreateStudentResponse>.ErrorResponse("Invalid gender value", 400);

                }

                var student = new ApplicationUser
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

                var createResult = await _userManager.CreateAsync(student);
                if (!createResult.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken);

                    return EndpointResponse<CreateStudentResponse>.ErrorResponse(
                        "Failed to create student",
                        400,
                        createResult.Errors.Select(e => e.Description).ToList()
                    );
                }

                // 3️⃣ Assign Student role
                var roleResult = await _userManager.AddToRoleAsync(student, "Student");
                if (!roleResult.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken);

                    return EndpointResponse<CreateStudentResponse>.ErrorResponse(
                        "Failed to assign student role",
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
                    UserId = student.Id,
                    Role = "Student",
                    ExpiryDate = DateTime.UtcNow.AddDays(3),
                    IsUsed = false
                };

                _context.ActivationCodes.Add(activationCode);
                await _context.SaveChangesAsync(cancellationToken);

                // 5️⃣ Commit transaction
                await transaction.CommitAsync(cancellationToken);

                // 6️⃣ Audit log (outside transaction)
                await _auditLogger.LogAsync(
                    action: "CreateStudent",
                    targetId: student.Id.ToString(),
                    description: $"Student created with email {student.Email}"
                );

                return EndpointResponse<CreateStudentResponse>.SuccessResponse(
                    new CreateStudentResponse
                    {
                        StudentId = student.Id,
                        Email = student.Email!,
                        ActivationCode = code
                    },
                    "Student created successfully",
                    201
                );
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                Log.Error(ex, "CreateStudent failed for {Email}", request.Email);

                return EndpointResponse<CreateStudentResponse>.ErrorResponse(
                    "Unexpected error occurred while creating student",
                    500
                );
            }
        }
    }
}
