using Auth.Contarcts;
using Auth.Models;
using Auth_Service.Features.Shared;
using AuthService.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AuthService.Features.Auth.Activate
{
    public class ActivateHandler
      : IRequestHandler<ActivateCommand, EndpointResponse<ActivateResponse>>
    {
        private readonly UniversitySystemAuthContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemoryCache? _cache;
        private readonly ITokenService _tokenService;

        public ActivateHandler(
            UniversitySystemAuthContext context,
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
              IMemoryCache? cache = null)
        {
            _context = context;
            _userManager = userManager;
            _tokenService = tokenService;
            _cache = cache;
        }

        public async Task<EndpointResponse<ActivateResponse>> Handle(
            ActivateCommand request,
            CancellationToken cancellationToken)
        {
            var activationCode = await _context.ActivationCodes
                .FirstOrDefaultAsync(x =>
                    x.Code == request.Code &&
                    !x.IsUsed &&
                    x.ExpiryDate > DateTime.UtcNow,
                    cancellationToken);

            if (activationCode == null)
            {
                return EndpointResponse<ActivateResponse>.NotFoundResponse(
                    "Invalid or expired activation code");
            }

            
            var user = await _userManager.FindByIdAsync(activationCode.UserId.ToString());
            if (user == null)
            {
                return EndpointResponse<ActivateResponse>.NotFoundResponse(
                    "User not found");
            }

            if (user.IsActivated)
            {
                return EndpointResponse<ActivateResponse>.ErrorResponse(
                    "Account already activated", 409);
            }

            var passwordResult = await _userManager.AddPasswordAsync(user, request.Password);
            if (!passwordResult.Succeeded)
            {
                return EndpointResponse<ActivateResponse>.ErrorResponse(
                    "Failed to set password",
                    400,
                    passwordResult.Errors.Select(e => e.Description).ToList()
                );
            }

            user.IsActivated = true;

            await _userManager.AddToRoleAsync(user, activationCode.Role);
            _cache.Remove($"roles_{user.Id}");

            activationCode.IsUsed = true;

            await _context.SaveChangesAsync(cancellationToken);

            var (accessToken, refreshToken) = await _tokenService.GenerateTokensAsync(user, rememberMe: false);

            return EndpointResponse<ActivateResponse>.SuccessResponse(
                new ActivateResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    Role = activationCode.Role
                },
                "Account activated successfully"
            );

        }
    }
}
