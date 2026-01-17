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
                .FirstOrDefaultAsync(x =>
                    x.Code == request.Code &&
                    !x.IsUsed &&
                    x.ExpiryDate > DateTime.UtcNow,
                    cancellationToken);

            if (parentCode == null)
            {
                return EndpointResponse<ActivateParentResponse>.NotFoundResponse(
                    "Invalid or expired parent code");
            }

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return EndpointResponse<ActivateParentResponse>.ErrorResponse(
                    "Account already exists",
                    409
                );
            }

            var parentUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = request.Email,
                Email = request.Email,
                EmailConfirmed = true,
                IsActivated = true
            };

            var createResult =
                await _userManager.CreateAsync(parentUser, request.Password);

            if (!createResult.Succeeded)
            {
                return EndpointResponse<ActivateParentResponse>.ErrorResponse(
                    "Failed to create parent account",
                    400,
                    createResult.Errors.Select(e => e.Description).ToList()
                );
            }

            await _userManager.AddToRoleAsync(parentUser, "Parent");

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
