using TaskManagementSystem.Application.Models;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Api.Models;

public static class ManagementUserMappings
{
    public static ManagementUserResponseDto ToResponseDto(this ManagementUser user)
    {
        return new ManagementUserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };
    }

    public static ManagementUserLoginResponseDto ToResponseDto(this AuthenticatedManagementUser user)
    {
        return new ManagementUserLoginResponseDto
        {
            AccessToken = user.AccessToken,
            TokenType = user.TokenType,
            ExpiresAt = user.ExpiresAt,
            User = new ManagementUserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            }
        };
    }
}
