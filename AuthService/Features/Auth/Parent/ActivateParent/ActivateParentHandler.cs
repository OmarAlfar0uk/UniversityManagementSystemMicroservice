using Auth.Contarcts;
using Auth.Models;
using Auth_Service.Features.Shared;
using AuthService.Data;
using AuthService.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Features.Auth.Parent.ActivateParent
{
    public class ActivateParentHandler : IRequestHandler<ActivateParentCommand, EndpointResponse<ActivateParentResponse>>
    {
        private readonly UniversitySystemAuthContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;

        public ActivateParentHandler(
            UniversitySystemAuthContext context,
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService)
        {
            _context = context;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<EndpointResponse<ActivateParentResponse>> Handle(
            ActivateParentCommand request,
            CancellationToken cancellationToken)
        {
            
            var parentCode = await _context.ParentCodes
                .FirstOrDefaultAsync(x => x.Code == request.Code && !x.IsUsed, cancellationToken);

            if (parentCode == null)
            {
                return EndpointResponse<ActivateParentResponse>.NotFoundResponse(
                    "Invalid activation code.");
            }

            if (parentCode.ExpiryDate <= DateTime.UtcNow)
            {
                return EndpointResponse<ActivateParentResponse>.ErrorResponse(
                    "Activation code has expired. Please request a new one.",
                    400
                );
            }

            var existingUser = await _userManager.FindByEmailAsync(request.Email);

            ApplicationUser parentUser;

            if (existingUser != null)
            {
                if (existingUser.IsActivated)
                {
                    return EndpointResponse<ActivateParentResponse>.ErrorResponse(
                        "User already activated.",
                        409
                    );
                }

                parentUser = existingUser;
                parentUser.FirstName = request.FirstName;
                parentUser.LastName = request.LastName;
                parentUser.EmailConfirmed = true;
                parentUser.IsActivated = true;

                if (string.IsNullOrWhiteSpace(parentUser.UniversityId))
                {
                    string generatedUniversityId() => "PAR-" + Guid.NewGuid().ToString("N")[..10];

                    var newUniversityId = generatedUniversityId();
                    while (await _userManager.Users.AnyAsync(u => u.UniversityId == newUniversityId, cancellationToken))
                    {
                        newUniversityId = generatedUniversityId();
                    }

                    parentUser.UniversityId = newUniversityId;
                }

                var updateResult = await _userManager.UpdateAsync(parentUser);
                if (!updateResult.Succeeded)
                {
                    return EndpointResponse<ActivateParentResponse>.ErrorResponse(
                        "Failed to update parent account",
                        400,
                        updateResult.Errors.Select(e => e.Description).ToList()
                    );
                }

                if (!await _userManager.HasPasswordAsync(parentUser))
                {
                    var addPasswordResult = await _userManager.AddPasswordAsync(parentUser, request.Password);
                    if (!addPasswordResult.Succeeded)
                    {
                        return EndpointResponse<ActivateParentResponse>.ErrorResponse(
                            "Failed to set password for existing account",
                            400,
                            addPasswordResult.Errors.Select(e => e.Description).ToList()
                        );
                    }
                }

                await _userManager.AddToRoleAsync(parentUser, "Parent");
            }
            else
            {
                string generatedUniversityId() => "PAR-" + Guid.NewGuid().ToString("N")[..10];

                var newUniversityId = generatedUniversityId();
                while (await _userManager.Users.AnyAsync(u => u.UniversityId == newUniversityId, cancellationToken))
                {
                    newUniversityId = generatedUniversityId();
                }

                parentUser = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = request.Email,
                    Email = request.Email,
                    EmailConfirmed = true,
                    IsActivated = true,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    UniversityId = newUniversityId
                };

                var createResult = await _userManager.CreateAsync(parentUser, request.Password);
                if (!createResult.Succeeded)
                {
                    return EndpointResponse<ActivateParentResponse>.ErrorResponse(
                        "Failed to create parent account",
                        400,
                        createResult.Errors.Select(e => e.Description).ToList()
                    );
                }

                await _userManager.AddToRoleAsync(parentUser, "Parent");
            }

            var link = new ParentStudent
            {
                Id = Guid.NewGuid(),
                ParentId = parentUser.Id,
                StudentId = parentCode.StudentId
            };

            _context.ParentStudents.Add(link);

            parentCode.IsUsed = true;

            await _context.SaveChangesAsync(cancellationToken);

            var (accessToken, refreshToken) =
                await _tokenService.GenerateTokensAsync(parentUser, rememberMe: false);

            return EndpointResponse<ActivateParentResponse>.SuccessResponse(
                new ActivateParentResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    Role = "Parent"
                },
                "Parent account activated successfully"
            );
        }
    }
}
