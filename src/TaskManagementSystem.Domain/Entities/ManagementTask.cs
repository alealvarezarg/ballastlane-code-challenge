using TaskManagementSystem.Domain.Enums;
using TaskManagementSystem.Domain.Exceptions;

namespace TaskManagementSystem.Domain.Entities;

public sealed class ManagementTask
{
    public ManagementTask(
        Guid id,
        string title,
        string description,
        ManagementTaskStatus status,
        DateTime dueDate,
        Guid userId,
        bool isArchived = false,
        DateTime? archivedAt = null)
    {
        Id = ValidateIdentifier(id, nameof(Id));
        ApplyInformation(title, description, dueDate, userId);
        Status = ValidateStatus(status);
        IsArchived = isArchived;
        ArchivedAt = archivedAt;
    }

    public Guid Id { get; }

    public string Title { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public ManagementTaskStatus Status { get; private set; }

    public DateTime DueDate { get; private set; }

    public Guid UserId { get; private set; }

    public bool IsArchived { get; private set; }

    public DateTime? ArchivedAt { get; private set; }

    public static ManagementTask CreateNew(
        Guid id,
        string title,
        string description,
        ManagementTaskStatus? status,
        DateTime dueDate,
        Guid userId)
    {
        return new ManagementTask(
            id,
            title,
            description,
            status ?? ManagementTaskStatus.Pending,
            dueDate,
            userId);
    }

    public void UpdateInformation(string title, string description, DateTime dueDate, Guid userId)
    {
        EnsureEditable();
        ApplyInformation(title, description, dueDate, userId);
    }

    public void ApplyFullUpdate(
        string title,
        string description,
        ManagementTaskStatus status,
        DateTime dueDate,
        Guid userId)
    {
        EnsureEditable();
        ApplyInformation(title, description, dueDate, userId);

        if (Status != status)
        {
            ChangeStatus(status);
        }
    }

    public void PatchInformation(string? title, string? description, DateTime? dueDate, Guid? userId)
    {
        EnsureEditable();

        if (title is null && description is null && dueDate is null && userId is null)
        {
            throw new InvalidManagementTaskException("At least one mutable field must be supplied.");
        }

        if (title is not null)
        {
            Title = ValidateRequiredText(title, nameof(Title));
        }

        if (description is not null)
        {
            Description = ValidateRequiredText(description, nameof(Description));
        }

        if (dueDate.HasValue)
        {
            DueDate = ValidateDueDate(dueDate.Value);
        }

        if (userId.HasValue)
        {
            UserId = ValidateIdentifier(userId.Value, nameof(UserId));
        }
    }

    public void ChangeStatus(ManagementTaskStatus status)
    {
        EnsureNotArchived();

        var validatedStatus = ValidateStatus(status);

        if (Status == validatedStatus)
        {
            return;
        }

        if (!IsTransitionAllowed(Status, validatedStatus))
        {
            throw new ManagementTaskLifecycleException(
                $"Status transition from '{Status}' to '{validatedStatus}' is not allowed.");
        }

        Status = validatedStatus;
    }

    public void Archive(DateTime archivedAt)
    {
        EnsureNotArchived();

        if (Status == ManagementTaskStatus.Completed)
        {
            throw new ManagementTaskLifecycleException(
                "Completed tasks cannot be deleted.");
        }

        IsArchived = true;
        ArchivedAt = ValidateDueDate(archivedAt);
    }

    private void ApplyInformation(string title, string description, DateTime dueDate, Guid userId)
    {
        Title = ValidateRequiredText(title, nameof(Title));
        Description = ValidateRequiredText(description, nameof(Description));
        DueDate = ValidateDueDate(dueDate);
        UserId = ValidateIdentifier(userId, nameof(UserId));
    }

    private void EnsureEditable()
    {
        EnsureNotArchived();

        if (Status == ManagementTaskStatus.Completed)
        {
            throw new ManagementTaskLifecycleException(
                "Completed tasks cannot be modified.");
        }
    }

    private void EnsureNotArchived()
    {
        if (IsArchived)
        {
            throw new ManagementTaskLifecycleException(
                "Archived tasks cannot be modified.");
        }
    }

    private static bool IsTransitionAllowed(ManagementTaskStatus current, ManagementTaskStatus next)
    {
        return (current, next) switch
        {
            (ManagementTaskStatus.Pending, ManagementTaskStatus.InProgress) => true,
            (ManagementTaskStatus.Pending, ManagementTaskStatus.Completed) => true,
            (ManagementTaskStatus.InProgress, ManagementTaskStatus.Completed) => true,
            _ => false
        };
    }

    private static ManagementTaskStatus ValidateStatus(ManagementTaskStatus status)
    {
        if (!Enum.IsDefined(status))
        {
            throw new InvalidManagementTaskException($"{nameof(Status)} must be a valid task status.");
        }

        return status;
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
