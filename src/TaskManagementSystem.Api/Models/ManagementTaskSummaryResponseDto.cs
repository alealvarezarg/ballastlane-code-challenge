namespace TaskManagementSystem.Api.Models;

public sealed class ManagementTaskSummaryResponseDto
{
    public IReadOnlyCollection<ManagementTaskStatusCountDto> Statuses { get; set; } = Array.Empty<ManagementTaskStatusCountDto>();
}
