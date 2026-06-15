namespace TaskManagementSystem.Domain.Exceptions;

public sealed class ManagementTaskLifecycleException : DomainException
{
    public ManagementTaskLifecycleException(string message)
        : base(message)
    {
    }
}
