namespace TaskManagementSystem.Domain.Exceptions;

public sealed class InvalidManagementUserException : DomainValidationException
{
    public InvalidManagementUserException(string message)
        : base(message)
    {
    }
}
