using Auth.Contarcts;
using Auth.Models;
using Auth_Service.Features.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Features.Auth.ForgotPassword
{
    public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, EndpointResponse<ForgotPasswordResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMailKitEmailService _emailService;
        private readonly IMemoryCache _cache;

        public ForgotPasswordHandler(
            UserManager<ApplicationUser> userManager,
            IMailKitEmailService emailService,
            IMemoryCache cache)
        {
            _userManager = userManager;
            _emailService = emailService;
            _cache = cache;
        }

        public async Task<EndpointResponse<ForgotPasswordResponse>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Verify User
                var user = request.EmailOrId.Contains("@")
                    ? await _userManager.FindByEmailAsync(request.EmailOrId)
                    : await _userManager.Users.SingleOrDefaultAsync(u => u.UniversityId == request.EmailOrId, cancellationToken);
                    
                if (user == null || string.IsNullOrEmpty(user.Email))
                {
                    // Security: Prevent enumeration attacks
                    return EndpointResponse<ForgotPasswordResponse>.SuccessResponse(
                        new ForgotPasswordResponse
                        {
                            Message = "If the account exists, an OTP has been sent.",
                            MaskedEmail = "",
                            RetryAfterSeconds = 30
                        },
                        "Success"
                    );
                }

                // 2. Rate Limiting (30 seconds cooldown)
                string rateLimitKey = $"ratelimit_otp_{user.Id}";
                if (_cache.TryGetValue(rateLimitKey, out _))
                {
                    return EndpointResponse<ForgotPasswordResponse>.ErrorResponse("Please wait 30 seconds before requesting a new OTP.", 429);
                }

                // 3. Generate & Hash OTP
                var otp = new Random().Next(100000, 999999).ToString();
                var hashedOtp = SecurityHelper.HashOtp(otp);

                // 4. Store Hash with 10-minute expiration
                string otpCacheKey = $"otp_{user.Id}";
                _cache.Set(otpCacheKey, hashedOtp, TimeSpan.FromMinutes(10));
                
                // Block new requests for 30 seconds
                _cache.Set(rateLimitKey, true, TimeSpan.FromSeconds(30));

                // 5. Send Email
                await _emailService.SendOtpEmailAsync(user.Email, otp);

                return EndpointResponse<ForgotPasswordResponse>.SuccessResponse(
                    new ForgotPasswordResponse 
                    { 
                        MaskedEmail = SecurityHelper.MaskEmail(user.Email),
                        RetryAfterSeconds = 30,
                        Message = "OTP sent successfully."
                    },
                    "Success"
                );
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to process forgot password request for {Identifier}", request.EmailOrId);
                return EndpointResponse<ForgotPasswordResponse>.ErrorResponse("An error occurred while processing your request.", 500);
            }
        }
    }
}
