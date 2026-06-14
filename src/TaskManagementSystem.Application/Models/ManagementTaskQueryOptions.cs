using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.Models;

public sealed class ManagementTaskQueryOptions
{
    public ManagementTaskStatus? Status { get; init; }

    public Guid? UserId { get; init; }

    public string? Search { get; init; }

    public ManagementTaskSortField SortBy { get; init; } = ManagementTaskSortField.DueDate;

    public SortDirection SortDirection { get; init; } = SortDirection.Ascending;

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 50;

    public bool IncludeArchived { get; init; }
}
