namespace TaskManagementSystem.Domain.Exceptions;

public sealed class DuplicateManagementUserException : DomainException
{
    public DuplicateManagementUserException(string message)
        : base(message)
    {
    }
}
