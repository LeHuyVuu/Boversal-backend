using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProjectManagementService.Application.Interfaces;
using ProjectManagementService.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectManagementService.Infrastructure.Services;

// Service tạo và validate JWT token
public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            Environment.GetEnvironmentVariable("JWT_KEY")
                ?? "your-super-secret-key-min-32-characters-long-12345"));
        
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "ProjectManagementAPI",
            audience: Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "ProjectManagementClient",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(
                int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRATION_HOURS") ?? "24")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public long? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(
                Environment.GetEnvironmentVariable("Jwt__Key") 
                    ?? _configuration["Jwt:Key"] 
                    ?? "your-super-secret-key-min-32-characters-long-12345");

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = Environment.GetEnvironmentVariable("Jwt__Issuer") 
                    ?? _configuration["Jwt:Issuer"] 
                    ?? "ProjectManagementAPI",
                ValidateAudience = true,
                ValidAudience = Environment.GetEnvironmentVariable("Jwt__Audience") 
                    ?? _configuration["Jwt:Audience"] 
                    ?? "ProjectManagementClient",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = long.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

            return userId;
        }
        catch
        {
            return null;
        }
    }
}
