using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Infrastructure.Repositories;

public sealed class ManagementTaskDocument
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ManagementTaskStatus Status { get; set; }

    public DateTime DueDate { get; set; }

    public Guid UserId { get; set; }

    public static ManagementTaskDocument FromDomain(ManagementTask task)
    {
        return new ManagementTaskDocument
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            DueDate = task.DueDate,
            UserId = task.UserId
        };
    }

    public ManagementTask ToDomain()
    {
        return new ManagementTask(Id, Title, Description, Status, DueDate, UserId);
    }
}
