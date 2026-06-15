using Microsoft.Extensions.Options;
using Shouldly;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;
using TaskManagementSystem.Infrastructure.Database;
using TaskManagementSystem.Infrastructure.Repositories;

namespace TaskManagementSystem.Infrastructure.UnitTests.Repositories;

public sealed class ManagementTaskRepositoryTests : IDisposable
{
    private readonly LiteDbContext _context;
    private readonly string _databasePath;
    private readonly ManagementTaskRepository _repository;

    public ManagementTaskRepositoryTests()
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
    public async Task CreateAsync_ShouldPersistTask()
    {
        var task = CreateTask();

        var result = await _repository.CreateAsync(task);

        result.Id.ShouldBe(task.Id);
        var stored = await _repository.GetByIdAsync(task.Id);
        stored.ShouldNotBeNull();
        stored.Title.ShouldBe(task.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenTaskDoesNotExist()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTasks()
    {
        var firstTask = CreateTask();
        var secondTask = CreateTask();

        await _repository.CreateAsync(firstTask);
        await _repository.CreateAsync(secondTask);

        var result = await _repository.GetAllAsync();

        result.Count.ShouldBe(2);
        result.Select(task => task.Id).ShouldContain(firstTask.Id);
        result.Select(task => task.Id).ShouldContain(secondTask.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistUpdatedTask()
    {
        var task = CreateTask();
        await _repository.CreateAsync(task);
        task.UpdateInformation("Updated title", "Updated description", DateTime.UtcNow.AddDays(5), Guid.NewGuid());

        var result = await _repository.UpdateAsync(task);

        result.Title.ShouldBe("Updated title");
        var stored = await _repository.GetByIdAsync(task.Id);
        stored.ShouldNotBeNull();
        stored.Title.ShouldBe("Updated title");
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenTaskDoesNotExist()
    {
        var task = CreateTask();

        await Should.ThrowAsync<KeyNotFoundException>(() => _repository.UpdateAsync(task));
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveTask_WhenTaskExists()
    {
        var task = CreateTask();
        await _repository.CreateAsync(task);

        await _repository.DeleteAsync(task.Id);

        var stored = await _repository.GetByIdAsync(task.Id);
        stored.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldNotThrow_WhenTaskDoesNotExist()
    {
        await Should.NotThrowAsync(() => _repository.DeleteAsync(Guid.NewGuid()));
    }

    public void Dispose()
    {
        _context.Dispose();

        if (File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }
    }

    private static ManagementTask CreateTask()
    {
        return new ManagementTask(
            Guid.NewGuid(),
            "Task title",
            "Task description",
            ManagementTaskStatus.Pending,
            DateTime.UtcNow.AddDays(2),
            Guid.NewGuid());
    }
}
