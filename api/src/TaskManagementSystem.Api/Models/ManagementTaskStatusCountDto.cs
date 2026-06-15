using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Api.Models;

public sealed class ManagementTaskStatusCountDto
{
    public ManagementTaskStatus Status { get; set; }

    public int Count { get; set; }
}
