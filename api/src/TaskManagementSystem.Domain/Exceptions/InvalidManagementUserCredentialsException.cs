namespace TaskManagementSystem.Domain.Exceptions;

public sealed class InvalidManagementUserCredentialsException : DomainException
{
    public InvalidManagementUserCredentialsException(string message)
        : base(message)
    {
    }
}
