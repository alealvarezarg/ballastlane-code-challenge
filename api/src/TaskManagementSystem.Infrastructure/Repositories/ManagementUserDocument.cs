using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Infrastructure.Repositories;

public sealed class ManagementUserDocument
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public static ManagementUserDocument FromDomain(ManagementUser user)
    {
        return new ManagementUserDocument
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            PasswordHash = user.PasswordHash
        };
    }

    public ManagementUser ToDomain()
    {
        return new ManagementUser(Id, Username, Email, PasswordHash);
    }
}
