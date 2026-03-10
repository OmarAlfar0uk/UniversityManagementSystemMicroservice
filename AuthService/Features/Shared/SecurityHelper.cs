using System.Security.Cryptography;
using System.Text;

namespace Auth_Service.Features.Shared
{
    public static class SecurityHelper
    {
        public static string HashOtp(string otp)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(otp);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static string MaskEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains('@')) return email;

            var parts = email.Split('@');
            if (parts.Length != 2) return email;

            var name = parts[0];
            var domain = parts[1];

            if (name.Length <= 2) return $"*@{domain}";

            return $"{name[0]}***{name[^1]}@{domain}";
        }
    }
}
