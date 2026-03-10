using Auth.Models;
using Auth_Service.Features.Shared;
using AuthService.Contracts;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Features.Auth.Admin.UpdateUserEmail
{
    public class UpdateUserEmailHandler
        : IRequestHandler<UpdateUserEmailCommand, EndpointResponse<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthAuditLogger _auditLogger;

        public UpdateUserEmailHandler(
            UserManager<ApplicationUser> userManager,
            IAuthAuditLogger auditLogger)
        {
            _userManager = userManager;
            _auditLogger = auditLogger;
        }

        public async Task<EndpointResponse<string>> Handle(
            UpdateUserEmailCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return EndpointResponse<string>.NotFoundResponse("User not found");

            // التأكد إن الإيميل الجديد مش موجود عند حد تاني
            var existingUser = await _userManager.FindByEmailAsync(request.NewEmail);
            if (existingUser != null && existingUser.Id != user.Id)
                return EndpointResponse<string>.ErrorResponse(
                    "Email is already in use by another account", 409);

            // SetEmailAsync بيحدّث Email + NormalizedEmail + UserName + NormalizedUserName تلقائياً
            var result = await _userManager.SetEmailAsync(user, request.NewEmail);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return EndpointResponse<string>.ErrorResponse(
                    "Failed to update email", 400, errors);
            }

            // تحديث UserName كمان عشان Identity بيستخدمه في الـ login
            await _userManager.SetUserNameAsync(user, request.NewEmail);

            await _auditLogger.LogAsync(
                action: "UpdateUserEmail",
                targetId: request.UserId.ToString());

            return EndpointResponse<string>.SuccessResponse(
                $"Email updated to {request.NewEmail}",
                "Email updated successfully");
        }
    }
}
