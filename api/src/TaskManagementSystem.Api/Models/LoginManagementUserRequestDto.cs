namespace TaskManagementSystem.Api.Models;

public sealed class LoginManagementUserRequestDto
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
