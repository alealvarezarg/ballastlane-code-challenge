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
        Users = _database.GetCollection<ManagementUserDocument>(settings.UsersCollectionName);
        IdempotencyRecords = _database.GetCollection<IdempotencyRecordDocument>("management_task_idempotency");
        Tasks.EnsureIndex(task => task.Id, unique: true);
        Users.EnsureIndex(user => user.Id, unique: true);
        Users.EnsureIndex(user => user.Username);
        Users.EnsureIndex(user => user.Email);
        IdempotencyRecords.EnsureIndex(record => record.Key, unique: true);
    }

    public LiteDbSettings Settings { get; }

    public ILiteCollection<ManagementTaskDocument> Tasks { get; }

    public ILiteCollection<ManagementUserDocument> Users { get; }

    public ILiteCollection<IdempotencyRecordDocument> IdempotencyRecords { get; }

    public void Dispose()
    {
        _database.Dispose();
    }
}
