using Auth.Models;
using Auth_Service.Features.Shared;
using AuthService.Contracts;
using AuthService.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Features.Auth.Admin.DeleteUser
{
    public class DeleteUserHandler
        : IRequestHandler<DeleteUserCommand, EndpointResponse<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuditLogger _auditLogger;
        private readonly UniversitySystemAuthContext _context;

        public DeleteUserHandler(
            UserManager<ApplicationUser> userManager,
            IAuditLogger auditLogger,
            UniversitySystemAuthContext context)
        {
            _userManager = userManager;
            _auditLogger = auditLogger;
            _context = context;
        }

        public async Task<EndpointResponse<string>> Handle(
            DeleteUserCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return EndpointResponse<string>.NotFoundResponse("User not found");

            // 🔐 Safety: منع حذف SuperAdmin
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("SuperAdmin"))
                return EndpointResponse<string>.ErrorResponse(
                    "Cannot delete a SuperAdmin account", 403);

            // 1️⃣ امسح الـ ActivationCodes المرتبطة باليوزر أولاً (FK constraint)
            var activationCodes = await _context.ActivationCodes
                .Where(a => a.UserId == user.Id)
                .ToListAsync(cancellationToken);

            if (activationCodes.Any())
            {
                _context.ActivationCodes.RemoveRange(activationCodes);
                await _context.SaveChangesAsync(cancellationToken);
            }

            // 2️⃣ دلوقتي امسح اليوزر بأمان
            // UserManager.DeleteAsync بيشيل اليوزر من كل جداول Identity تلقائياً
            // (AspNetUserRoles, AspNetUserClaims, AspNetUserLogins, AspNetUserTokens)
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return EndpointResponse<string>.ErrorResponse(
                    "Failed to delete user", 400, errors);
            }

            await _auditLogger.LogAsync(
                action: "DeleteUser",
                targetId: request.UserId.ToString());

            return EndpointResponse<string>.SuccessResponse(
                "User deleted successfully",
                "User deleted successfully");
        }
    }
}
