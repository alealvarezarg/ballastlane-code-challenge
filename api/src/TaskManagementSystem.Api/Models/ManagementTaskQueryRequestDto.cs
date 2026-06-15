using TaskManagementSystem.Application.Models;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Api.Models;

public sealed class ManagementTaskQueryRequestDto
{
    public ManagementTaskStatus? Status { get; set; }

    public Guid? UserId { get; set; }

    public string? Search { get; set; }

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
            Page = Page,
            PageSize = PageSize,
            IncludeArchived = IncludeArchived
        };
    }
}
