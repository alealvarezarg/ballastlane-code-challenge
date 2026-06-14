namespace TaskManagementSystem.Infrastructure.Authentication;

public sealed class JwtAuthenticationSettings
{
    public const string SectionName = "Authentication";

    public string Issuer { get; set; } = "TaskManagementSystem";

    public string Audience { get; set; } = "TaskManagementSystem.Api";

    public string SigningKey { get; set; } = "TaskManagementSystemDevelopmentSigningKey123!";

    public int AccessTokenLifetimeMinutes { get; set; } = 60;
}
