using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Api.Models;

public sealed class ManagementTaskResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ManagementTaskStatus Status { get; set; }

    public DateTime DueDate { get; set; }

    public Guid UserId { get; set; }

    public bool IsArchived { get; set; }

    public DateTime? ArchivedAt { get; set; }
}
