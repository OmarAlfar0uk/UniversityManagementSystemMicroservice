using Auth.Contarcts;
using Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Auth.Repositories
{
    public class JwtService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemoryCache _cache;

        public JwtService(IConfiguration config, UserManager<ApplicationUser> userManager, IMemoryCache cache)
        {
            _config = config;
            _userManager = userManager;
            _cache = cache;
        }

        // ========================= 🔐 Generate Tokens =========================
        public async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(ApplicationUser user, bool rememberMe)
        {
            var roles = await GetUserRolesCachedAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("id", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            if (user.DepartmentId.HasValue)
                claims.Add(new Claim("DepartmentId", user.DepartmentId.Value.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var accessToken = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(GetAccessTokenExpiryMinutes()),
                signingCredentials: creds
            );
            var accessTokenString = new JwtSecurityTokenHandler().WriteToken(accessToken);

            // Refresh Token
            var refreshToken = GenerateSecureRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(
                rememberMe ? GetRememberMeRefreshExpiryDays() : GetDefaultRefreshExpiryDays());

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = refreshTokenExpiry;
            await EnsureUpdateSucceededAsync(user);

            return (accessTokenString, refreshToken);
        }

        // ========================= 🔁 Refresh Access Token =========================
        public async Task<(string AccessToken, string RefreshToken)?> RefreshAccessTokenAsync(string refreshToken)
        {
            var user = await _userManager.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                return null;

            var trackedUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (trackedUser == null)
                return null;

            var roles = await GetUserRolesCachedAsync(trackedUser);

            var newAccessToken = GenerateAccessToken(trackedUser, roles);

            // Renew Refresh Token
            var newRefreshToken = GenerateSecureRefreshToken();
            trackedUser.RefreshToken = newRefreshToken;

            var wasRememberMeSession =
                trackedUser.RefreshTokenExpiryTime.HasValue &&
                trackedUser.RefreshTokenExpiryTime.Value > DateTime.UtcNow.AddDays(GetDefaultRefreshExpiryDays());
            trackedUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                wasRememberMeSession ? GetRememberMeRefreshExpiryDays() : GetDefaultRefreshExpiryDays());

            await EnsureUpdateSucceededAsync(trackedUser);

            return (newAccessToken, newRefreshToken);
        }

        // ========================= 🔐 Access Token Generator =========================
        private string GenerateAccessToken(ApplicationUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("id", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (!string.IsNullOrEmpty(user.Email))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            }

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            if (user.DepartmentId.HasValue)
                claims.Add(new Claim("DepartmentId", user.DepartmentId.Value.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(GetAccessTokenExpiryMinutes()),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ========================= ❌ Revoke Refresh Token =========================
        public async Task RevokeRefreshTokenAsync(ApplicationUser user)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await EnsureUpdateSucceededAsync(user);
        }

        private static string GenerateSecureRefreshToken()
        {
            return Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(64));
        }

        private int GetAccessTokenExpiryMinutes()
        {
            return _config.GetValue<int?>("JwtSettings:AccessTokenExpiryMinutes") ?? 30;
        }

        private int GetDefaultRefreshExpiryDays()
        {
            return _config.GetValue<int?>("JwtSettings:RefreshTokenExpiryDays") ?? 7;
        }

        private int GetRememberMeRefreshExpiryDays()
        {
            return _config.GetValue<int?>("JwtSettings:RememberMeRefreshTokenExpiryDays") ?? 30;
        }

        private async Task EnsureUpdateSucceededAsync(ApplicationUser user)
        {
            var updateResult = await _userManager.UpdateAsync(user);
            if (updateResult.Succeeded)
            {
                return;
            }

            var reason = string.Join("; ", updateResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to persist refresh token: {reason}");
        }

        // ========================= ⭐ Cached Roles Logic =========================
        private async Task<IList<string>> GetUserRolesCachedAsync(ApplicationUser user)
        {
            string cacheKey = $"roles_{user.Id}";

            if (_cache.TryGetValue(cacheKey, out IList<string> cachedRoles))
                return cachedRoles;

            var roles = await _userManager.GetRolesAsync(user);

            _cache.Set(
                cacheKey,
                roles,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15),
                    Priority = CacheItemPriority.High
                }
            );

            return roles;
        }
    }
}
