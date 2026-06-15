namespace TaskManagementSystem.Api.Models;

public sealed class CreateManagementUserRequestDto
{
    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
