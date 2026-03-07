using Auth.Models;
using Auth_Service.Features.Shared;
using AuthService.Contracts;
using AuthService.Features.Auth.Admin.ToggleUserStatus;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Features.Admin.ToggleUserStatus
{
    public class ToggleUserStatusHandler
        : IRequestHandler<ToggleUserStatusCommand, EndpointResponse<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthAuditLogger _auditLogger;
        public ToggleUserStatusHandler(UserManager<ApplicationUser> userManager , IAuthAuditLogger auditLogger)
        {
            _userManager = userManager;
            _auditLogger = auditLogger;
        }

        public async Task<EndpointResponse<string>> Handle(
            ToggleUserStatusCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                return EndpointResponse<string>.NotFoundResponse("User not found");
            }

            // 🔐 Safety: منع قفل SuperAdmin
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("SuperAdmin") && !request.Enable)
            {
                return EndpointResponse<string>.ErrorResponse(
                    "Cannot disable SuperAdmin",
                    403
                );
            }

            // Toggle
            user.IsActivated = request.Enable;
            await _userManager.UpdateAsync(user);
            await _auditLogger.LogAsync(
    action: request.Enable ? "EnableUser" : "DisableUser",
    targetId: user.Id.ToString()
);

            var message = request.Enable
                ? "User enabled successfully"
                : "User disabled successfully";

            return EndpointResponse<string>.SuccessResponse(
                message,
                message
            );
        }
    }
}
