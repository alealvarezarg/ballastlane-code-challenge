namespace TaskManagementSystem.Api.Models;

public sealed class PatchManagementTaskRequestDto
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public Guid? UserId { get; set; }
}
