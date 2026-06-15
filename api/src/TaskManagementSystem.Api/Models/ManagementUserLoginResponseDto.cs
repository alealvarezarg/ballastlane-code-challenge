namespace TaskManagementSystem.Api.Models;

public sealed class ManagementUserLoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;

    public string TokenType { get; set; } = "Bearer";

    public DateTime ExpiresAt { get; set; }

    public ManagementUserResponseDto User { get; set; } = new();
}
