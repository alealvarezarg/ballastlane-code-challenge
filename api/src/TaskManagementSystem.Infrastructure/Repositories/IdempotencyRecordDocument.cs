using LiteDB;
using TaskManagementSystem.Application.Models;

namespace TaskManagementSystem.Infrastructure.Repositories;

public sealed class IdempotencyRecordDocument
{
    [BsonId]
    public string Key { get; set; } = string.Empty;

    public string RequestFingerprint { get; set; } = string.Empty;

    public Guid TaskId { get; set; }

    public DateTime CreatedAt { get; set; }

    public static IdempotencyRecordDocument FromModel(IdempotencyRecord record)
    {
        return new IdempotencyRecordDocument
        {
            Key = record.Key,
            RequestFingerprint = record.RequestFingerprint,
            TaskId = record.TaskId,
            CreatedAt = record.CreatedAt
        };
    }

    public IdempotencyRecord ToModel()
    {
        return new IdempotencyRecord
        {
            Key = Key,
            RequestFingerprint = RequestFingerprint,
            TaskId = TaskId,
            CreatedAt = CreatedAt
        };
    }
}
