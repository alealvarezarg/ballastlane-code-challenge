using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.Models;

public sealed class ManagementTaskStatusSummary
{
    public required ManagementTaskStatus Status { get; init; }

    public required int Count { get; init; }
}
