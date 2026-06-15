namespace TaskManagementSystem.Api.Models;

public sealed class PagedManagementTaskResponseDto
{
    public IReadOnlyCollection<ManagementTaskResponseDto> Items { get; set; } = Array.Empty<ManagementTaskResponseDto>();

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }
}
