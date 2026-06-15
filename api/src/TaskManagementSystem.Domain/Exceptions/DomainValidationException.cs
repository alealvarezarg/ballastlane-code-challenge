namespace TaskManagementSystem.Domain.Exceptions;

public abstract class DomainValidationException : DomainException
{
    protected DomainValidationException(string message)
        : base(message)
    {
    }
}
