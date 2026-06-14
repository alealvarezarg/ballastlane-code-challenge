namespace TaskManagementSystem.Application.Models;

public sealed class ManagementTaskPatch
{
    public string? Title { get; init; }

    public string? Description { get; init; }

    public DateTime? DueDate { get; init; }

    public Guid? UserId { get; init; }
}
