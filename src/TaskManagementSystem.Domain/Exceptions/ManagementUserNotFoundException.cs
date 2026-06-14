namespace TaskManagementSystem.Domain.Exceptions;

public sealed class ManagementUserNotFoundException : DomainValidationException
{
    public ManagementUserNotFoundException(string message)
        : base(message)
    {
    }
}
