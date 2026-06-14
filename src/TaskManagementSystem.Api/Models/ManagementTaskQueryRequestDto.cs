using TaskManagementSystem.Application.Models;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Api.Models;

public sealed class ManagementTaskQueryRequestDto
{
    public ManagementTaskStatus? Status { get; set; }

    public Guid? UserId { get; set; }

    public string? Search { get; set; }

    public string? SortBy { get; set; }

    public string? SortDirection { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 50;

    public bool IncludeArchived { get; set; }

    public ManagementTaskQueryOptions ToQueryOptions()
    {
        return new ManagementTaskQueryOptions
        {
            Status = Status,
            UserId = UserId,
            Search = Search,
            SortBy = SortBy?.Trim().ToLowerInvariant() switch
            {
                "title" => ManagementTaskSortField.Title,
                "status" => ManagementTaskSortField.Status,
                _ => ManagementTaskSortField.DueDate
            },
            SortDirection = SortDirection?.Trim().ToLowerInvariant() == "desc"
                ? Application.Models.SortDirection.Descending
                : Application.Models.SortDirection.Ascending,
            Page = Page,
            PageSize = PageSize,
            IncludeArchived = IncludeArchived
        };
    }
}
