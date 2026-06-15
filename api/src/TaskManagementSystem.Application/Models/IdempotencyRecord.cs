namespace TaskManagementSystem.Application.Models;

public sealed class IdempotencyRecord
{
    public required string Key { get; init; }

    public required string RequestFingerprint { get; init; }

    public required Guid TaskId { get; init; }

    public required DateTime CreatedAt { get; init; }
}
