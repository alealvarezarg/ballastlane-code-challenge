using TaskManagementSystem.Domain.Enums;
using TaskManagementSystem.Domain.Exceptions;

namespace TaskManagementSystem.Domain.Entities;

public sealed class ManagementTask
{
    public ManagementTask(Guid id, string title, string description, ManagementTaskStatus status, DateTime dueDate, Guid userId)
    {
        Id = ValidateIdentifier(id, nameof(Id));
        UpdateInformation(title, description, dueDate, userId);
        ChangeStatus(status);
    }

    public Guid Id { get; }

    public string Title { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public ManagementTaskStatus Status { get; private set; }

    public DateTime DueDate { get; private set; }

    public Guid UserId { get; private set; }

    public void UpdateInformation(string title, string description, DateTime dueDate, Guid userId)
    {
        Title = ValidateRequiredText(title, nameof(Title));
        Description = ValidateRequiredText(description, nameof(Description));
        DueDate = ValidateDueDate(dueDate);
        UserId = ValidateIdentifier(userId, nameof(UserId));
    }

    public void ChangeStatus(ManagementTaskStatus status)
    {
        if (!Enum.IsDefined(status))
        {
            throw new InvalidManagementTaskException($"{nameof(Status)} must be a valid task status.");
        }

        Status = status;
    }

    private static string ValidateRequiredText(string value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidManagementTaskException($"{propertyName} cannot be null or empty.");
        }

        return value.Trim();
    }

    private static DateTime ValidateDueDate(DateTime dueDate)
    {
        if (dueDate == default)
        {
            throw new InvalidManagementTaskException($"{nameof(DueDate)} must be a valid date.");
        }

        return dueDate;
    }

    private static Guid ValidateIdentifier(Guid value, string propertyName)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidManagementTaskException($"{propertyName} must be a valid identifier.");
        }

        return value;
    }
}
