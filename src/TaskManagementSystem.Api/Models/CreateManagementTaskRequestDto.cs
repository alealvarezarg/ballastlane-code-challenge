using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Api.Models;

public sealed class CreateManagementTaskRequestDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ManagementTaskStatus? Status { get; set; }

    public DateTime DueDate { get; set; }

    public Guid UserId { get; set; }
}
