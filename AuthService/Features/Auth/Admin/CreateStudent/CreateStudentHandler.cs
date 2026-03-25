using Auth.Contarcts;
using Auth.Models;
using Auth_Service.Features.Shared;
using AuthService.Contracts;
using AuthService.Data;
using AuthService.Models;
using MassTransit;
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
        private readonly IAuthAuditLogger _auditLogger;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMailKitEmailService _emailService;


        public CreateStudentHandler(
       UserManager<ApplicationUser> userManager,
       UniversitySystemAuthContext context,
       IAuthAuditLogger auditLogger,
       IPublishEndpoint publishEndpoint,
       IMailKitEmailService emailService)
        {
            _userManager = userManager;
            _context = context;
            _auditLogger = auditLogger;
            _publishEndpoint = publishEndpoint;
            _emailService = emailService;
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

            if (!Enum.TryParse<Gender>(request.Gender, true, out var gender))
            {
                return EndpointResponse<CreateStudentResponse>.ErrorResponse("Invalid gender value", 400);
            }

            // ── DB Transaction scope ─────────────────────────────────────────────
            ApplicationUser student;
            string code;

            await using var transaction =
                await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Generate a sequential UniversityId (e.g., last 2 digits of Year + "001", "002", etc.)
                string yearSuffix = DateTime.UtcNow.Year.ToString().Substring(2, 2);

                var maxIdString = await _context.Users
                    .Where(u => !string.IsNullOrEmpty(u.UniversityId) && u.UniversityId.StartsWith(yearSuffix))
                    .OrderByDescending(u => u.UniversityId.Length)
                    .ThenByDescending(u => u.UniversityId)
                    .Select(u => u.UniversityId)
                    .FirstOrDefaultAsync(cancellationToken);

                string generatedUniversityId;
                if (string.IsNullOrEmpty(maxIdString))
                {
                    generatedUniversityId = $"{yearSuffix}001";
                }
                else
                {
                    var numericPart = maxIdString.Substring(2);
                    if (int.TryParse(numericPart, out int number))
                    {
                        generatedUniversityId = $"{yearSuffix}{(number + 1).ToString().PadLeft(3, '0')}";
                    }
                    else
                    {
                        // Fallback
                        generatedUniversityId = $"{yearSuffix}001";
                    }
                }

                // Ensure it's unique (in case of race conditions during concurrent requests)
                while (await _context.Users.AnyAsync(u => u.UniversityId == generatedUniversityId, cancellationToken))
                {
                    var numericPart = generatedUniversityId.Substring(2);
                    if (int.TryParse(numericPart, out int number))
                    {
                        generatedUniversityId = $"{yearSuffix}{(number + 1).ToString().PadLeft(3, '0')}";
                    }
                }

                student = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = request.Email,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Gender = gender,
                    IsActivated = false,
                    EmailConfirmed = false,
                    UniversityId = generatedUniversityId,
                    DepartmentId = request.DepartmentId
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

                code = Guid.NewGuid().ToString("N")[..6].ToUpper();

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

                await transaction.CommitAsync(cancellationToken);
                // ✅ Student is now safely in the database
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                Log.Error(ex, "CreateStudent DB transaction failed for {Email}", request.Email);
                return EndpointResponse<CreateStudentResponse>.ErrorResponse(
                    "Failed to create student due to a database error", 500);
            }

            // ── Post-commit side effects (fire & log — never rollback DB for these) ──
            try
            {
                await _emailService.SendActivationEmailAsync(
                    student.Email!,
                    $"{student.FirstName} {student.LastName}",
                    code,
                    "Student",
                    student.UniversityId
                );
            }
            catch (Exception emailEx)
            {
                // Student is already created — just log, don't fail the request
                Log.Warning(emailEx, "Activation email failed for {Email} (student still created)", request.Email);
            }

            try
            {
                await _publishEndpoint.Publish<IAuthCreated>(new
                {
                    UserId = student.Id,
                    Email = student.Email!,
                    UserName = student.UserName!,
                    Role = "Student",
                    CreatedAt = DateTime.UtcNow
                });
            }
            catch (Exception pubEx)
            {
                Log.Warning(pubEx, "MassTransit publish failed for student {Id} (student still created)", student.Id);
            }

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
                    ActivationCode = code,
                    UniversityId = student.UniversityId,
                    DepartmentId = student.DepartmentId
                },
                "Student created successfully",
                201
            );
        }
    }
}
