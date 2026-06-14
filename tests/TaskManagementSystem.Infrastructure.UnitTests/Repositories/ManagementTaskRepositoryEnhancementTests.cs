using Microsoft.Extensions.Options;
using Shouldly;
using TaskManagementSystem.Application.Models;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;
using TaskManagementSystem.Infrastructure.Database;
using TaskManagementSystem.Infrastructure.Repositories;

namespace TaskManagementSystem.Infrastructure.UnitTests.Repositories;

public sealed class ManagementTaskRepositoryEnhancementTests : IDisposable
{
    private readonly LiteDbContext _context;
    private readonly string _databasePath;
    private readonly ManagementTaskRepository _repository;

    public ManagementTaskRepositoryEnhancementTests()
    {
        _databasePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.db");
        var settings = new LiteDbSettings
        {
            ConnectionString = $"Filename={_databasePath};Connection=shared"
        };

        _context = new LiteDbContext(Options.Create(settings));
        _repository = new ManagementTaskRepository(_context);
    }

    [Fact]
    public async Task QueryAsync_ShouldFilterByStatusAndUser()
    {
        var userId = Guid.NewGuid();
        var matchingTask = CreateTask(userId, ManagementTaskStatus.Pending, "Matching title");
        var otherTask = CreateTask(Guid.NewGuid(), ManagementTaskStatus.Completed, "Other title");

        await _repository.CreateAsync(matchingTask);
        await _repository.CreateAsync(otherTask);

        var result = await _repository.QueryAsync(new ManagementTaskQueryOptions
        {
            Status = ManagementTaskStatus.Pending,
            UserId = userId,
            Page = 1,
            PageSize = 10
        });

        result.TotalCount.ShouldBe(1);
        result.Items.Single().Id.ShouldBe(matchingTask.Id);
    }

    [Fact]
    public async Task GetSummaryAsync_ShouldGroupByStatus()
    {
        await _repository.CreateAsync(CreateTask(Guid.NewGuid(), ManagementTaskStatus.Pending, "First"));
        await _repository.CreateAsync(CreateTask(Guid.NewGuid(), ManagementTaskStatus.Pending, "Second"));
        await _repository.CreateAsync(CreateTask(Guid.NewGuid(), ManagementTaskStatus.Completed, "Third"));

        var result = await _repository.GetSummaryAsync(includeArchived: false);

        result.ShouldContain(item => item.Status == ManagementTaskStatus.Pending && item.Count == 2);
        result.ShouldContain(item => item.Status == ManagementTaskStatus.Completed && item.Count == 1);
    }

    [Fact]
    public async Task SaveIdempotencyRecordAsync_ShouldPersistRecord()
    {
        var record = new IdempotencyRecord
        {
            Key = "idem-1",
            RequestFingerprint = "fingerprint",
            TaskId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        await _repository.SaveIdempotencyRecordAsync(record);
        var stored = await _repository.GetIdempotencyRecordAsync(record.Key);

        stored.ShouldNotBeNull();
        stored.TaskId.ShouldBe(record.TaskId);
    }

    public void Dispose()
    {
        _context.Dispose();

        if (File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }
    }

    private static ManagementTask CreateTask(Guid userId, ManagementTaskStatus status, string title)
    {
        return new ManagementTask(
            Guid.NewGuid(),
            title,
            "Task description",
            status,
            DateTime.UtcNow.AddDays(2),
            userId);
    }
}
