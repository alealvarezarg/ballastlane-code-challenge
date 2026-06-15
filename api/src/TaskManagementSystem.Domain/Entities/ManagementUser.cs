using TaskManagementSystem.Domain.Exceptions;

namespace TaskManagementSystem.Domain.Entities;

public sealed class ManagementUser
{
    public ManagementUser(Guid id, string username, string email, string passwordHash)
    {
        Id = ValidateIdentifier(id, nameof(Id));
        Username = ValidateRequiredText(username, nameof(Username));
        Email = ValidateRequiredText(email, nameof(Email));
        PasswordHash = ValidateRequiredText(passwordHash, nameof(PasswordHash));
    }

    public Guid Id { get; }

    public string Username { get; }

    public string Email { get; }

    public string PasswordHash { get; }

    private static string ValidateRequiredText(string value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidManagementUserException($"{propertyName} cannot be null or empty.");
        }

        return value.Trim();
    }

    private static Guid ValidateIdentifier(Guid value, string propertyName)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidManagementUserException($"{propertyName} must be a valid identifier.");
        }

        return value;
    }
}
