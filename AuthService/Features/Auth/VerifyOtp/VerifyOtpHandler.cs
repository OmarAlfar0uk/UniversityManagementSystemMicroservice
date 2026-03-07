using Auth_Service.Features.Shared;
using Auth.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Features.Auth.VerifyOtp
{
    public class VerifyOtpHandler : IRequestHandler<VerifyOtpCommand, EndpointResponse<VerifyOtpResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemoryCache _cache;

        public VerifyOtpHandler(UserManager<ApplicationUser> userManager, IMemoryCache cache)
        {
            _userManager = userManager;
            _cache = cache;
        }

        public async Task<EndpointResponse<VerifyOtpResponse>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            var user = request.EmailOrId.Contains("@")
                ? await _userManager.FindByEmailAsync(request.EmailOrId)
                : await _userManager.Users.SingleOrDefaultAsync(u => u.UniversityId == request.EmailOrId, cancellationToken);

            if (user == null) 
                return EndpointResponse<VerifyOtpResponse>.ErrorResponse("Invalid request.", 400);

            string otpCacheKey = $"otp_{user.Id}";
            
            // 1. Fetch Hashed OTP from Cache
            if (!_cache.TryGetValue(otpCacheKey, out string? storedHashedOtp))
            {
                return EndpointResponse<VerifyOtpResponse>.ErrorResponse("OTP has expired or does not exist.", 400);
            }

            // 2. Hash input and compare
            if (SecurityHelper.HashOtp(request.Otp) != storedHashedOtp)
            {
                return EndpointResponse<VerifyOtpResponse>.ErrorResponse("Invalid OTP.", 400);
            }

            // 3. OTP is valid! Invalidate it immediately to prevent reuse.
            _cache.Remove(otpCacheKey);

            // 4. Generate highly-secure Identity Reset Token
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            return EndpointResponse<VerifyOtpResponse>.SuccessResponse(
                new VerifyOtpResponse { ResetToken = resetToken }, 
                "OTP Verified.",
                200
            );
        }
    }
}
