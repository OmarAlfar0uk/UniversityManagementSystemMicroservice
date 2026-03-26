using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Learnify.Tests.Shared;

public static class JwtHelper
{
    private const string DefaultSecret  = "p@55w0rd_VeryStrong_SecretKey_2024_Learnify_University";
    private const string DefaultIssuer  = "LearnifyUniversity";
    private const string DefaultAudience = "LearnifyClients";

    public static string GenerateToken(
        Guid userId,
        string role,
        string secret   = DefaultSecret,
        string issuer   = DefaultIssuer,
        string audience = DefaultAudience)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role),
            new Claim("role", role)
        };

        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer:            issuer,
            audience:          audience,
            claims:            claims,
            expires:           DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string StudentToken(Guid? userId = null)
        => GenerateToken(userId ?? TestConstants.UserId, "Student");

    public static string DoctorToken(Guid? userId = null)
        => GenerateToken(userId ?? TestConstants.DoctorId, "Doctor");

    public static string AdminToken(Guid? userId = null)
        => GenerateToken(userId ?? TestConstants.DoctorId, "Admin");
}
