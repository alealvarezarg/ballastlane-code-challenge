using LiteDB;
using Microsoft.Extensions.Options;
using TaskManagementSystem.Infrastructure.Repositories;

namespace TaskManagementSystem.Infrastructure.Database;

public sealed class LiteDbContext : IDisposable
{
    private readonly LiteDatabase _database;

    public LiteDbContext(IOptions<LiteDbSettings> settingsOptions)
    {
        var settings = settingsOptions.Value;
        Settings = settings;
        _database = new LiteDatabase(settings.ConnectionString);
        Tasks = _database.GetCollection<ManagementTaskDocument>(settings.TasksCollectionName);
        IdempotencyRecords = _database.GetCollection<IdempotencyRecordDocument>("management_task_idempotency");
        Tasks.EnsureIndex(task => task.Id, unique: true);
        IdempotencyRecords.EnsureIndex(record => record.Key, unique: true);
    }

    public LiteDbSettings Settings { get; }

    public ILiteCollection<ManagementTaskDocument> Tasks { get; }

    public ILiteCollection<IdempotencyRecordDocument> IdempotencyRecords { get; }

    public void Dispose()
    {
        _database.Dispose();
    }
}
