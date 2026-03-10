using Auth.Models;
using Auth_Service.Features.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Features.Auth.ResetPassword
{
    public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, EndpointResponse<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ResetPasswordHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<EndpointResponse<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = request.EmailOrId.Contains("@")
                    ? await _userManager.FindByEmailAsync(request.EmailOrId)
                    : await _userManager.Users.SingleOrDefaultAsync(u => u.UniversityId == request.EmailOrId, cancellationToken);

                if (user == null) 
                    return EndpointResponse<string>.ErrorResponse("User not found.", 404);

                // Identity handles checking if the token is valid, matches the user, and hasn't expired maliciously.
                var result = await _userManager.ResetPasswordAsync(user, request.ResetToken, request.NewPassword);

                if (!result.Succeeded)
                {
                    return EndpointResponse<string>.ErrorResponse(
                        "Password reset failed. The token may be invalid, expired, or the password does not meet the policy.", 400, 
                        result.Errors.Select(e => e.Description).ToList());
                }

                return EndpointResponse<string>.SuccessResponse("Password has been successfully reset.", "Success", 200);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to reset password using token for {Identifier}", request.EmailOrId);
                return EndpointResponse<string>.ErrorResponse("An error occurred while resetting password.", 500);
            }
        }
    }
}
