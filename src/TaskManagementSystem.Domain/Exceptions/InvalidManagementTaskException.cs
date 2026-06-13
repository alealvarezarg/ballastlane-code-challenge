namespace TaskManagementSystem.Domain.Exceptions;

public sealed class InvalidManagementTaskException : DomainValidationException
{
    public InvalidManagementTaskException(string message)
        : base(message)
    {
    }
}
