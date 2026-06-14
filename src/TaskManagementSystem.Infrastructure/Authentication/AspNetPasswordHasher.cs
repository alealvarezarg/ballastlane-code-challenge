using TaskManagementSystem.Application.Abstractions;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Infrastructure.Authentication;

public sealed class AspNetPasswordHasher : IPasswordHasher
{
    private readonly Microsoft.AspNetCore.Identity.PasswordHasher<ManagementUser> _passwordHasher = new();

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(CreateTemplateUser(), password);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(
            CreateTemplateUser(),
            hashedPassword,
            providedPassword);

        return result != Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed;
    }

    private static ManagementUser CreateTemplateUser()
    {
        return new ManagementUser(Guid.NewGuid(), "template-user", "template@example.com", "template-hash");
    }
}
