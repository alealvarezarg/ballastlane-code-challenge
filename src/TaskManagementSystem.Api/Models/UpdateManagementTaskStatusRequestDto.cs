using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Api.Models;

public sealed class UpdateManagementTaskStatusRequestDto
{
    public ManagementTaskStatus Status { get; set; }
}
