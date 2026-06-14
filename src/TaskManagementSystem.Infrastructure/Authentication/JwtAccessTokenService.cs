using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskManagementSystem.Application.Abstractions;
using TaskManagementSystem.Application.Models;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Infrastructure.Authentication;

public sealed class JwtAccessTokenService : IAccessTokenService
{
    private readonly JwtAuthenticationSettings _settings;

    public JwtAccessTokenService(IOptions<JwtAuthenticationSettings> settingsOptions)
    {
        _settings = settingsOptions.Value;
    }

    public AuthenticatedManagementUser CreateToken(ManagementUser user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_settings.AccessTokenLifetimeMinutes);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SigningKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new AuthenticatedManagementUser
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt,
            TokenType = "Bearer"
        };
    }
}
