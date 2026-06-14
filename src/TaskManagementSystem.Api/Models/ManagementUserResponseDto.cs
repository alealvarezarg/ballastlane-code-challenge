namespace TaskManagementSystem.Api.Models;

public sealed class ManagementUserResponseDto
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}
