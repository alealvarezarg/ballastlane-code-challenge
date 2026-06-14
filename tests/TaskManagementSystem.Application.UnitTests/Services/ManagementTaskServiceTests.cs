using FakeItEasy;
using Microsoft.Extensions.Caching.Memory;
using Shouldly;
using TaskManagementSystem.Application.Abstractions;
using TaskManagementSystem.Application.Services;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.UnitTests.Services;

public sealed class ManagementTaskServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldCallRepositoryAndReturnCreatedTask()
    {
        var repository = A.Fake<IManagementTaskRepository>();
        using var memoryCache = CreateMemoryCache();
        var service = new ManagementTaskService(repository, memoryCache);
        var task = CreateTask();

        A.CallTo(() => repository.CreateAsync(task)).Returns(task);

        var result = await service.CreateAsync(task);

        result.ShouldBe(task);
        A.CallTo(() => repository.CreateAsync(task)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCachedTask_WhenCacheContainsValue()
    {
        var repository = A.Fake<IManagementTaskRepository>();
        var task = CreateTask();
        using var memoryCache = CreateMemoryCache();
        memoryCache.Set(GetTaskCacheKey(task.Id), task);
        var service = new ManagementTaskService(repository, memoryCache);

        var result = await service.GetByIdAsync(task.Id);

        result.ShouldBe(task);
        A.CallTo(() => repository.GetByIdAsync(task.Id)).MustNotHaveHappened();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldCacheRepositoryResult_WhenCacheMissOccurs()
    {
        var repository = A.Fake<IManagementTaskRepository>();
        var task = CreateTask();
        using var memoryCache = CreateMemoryCache();
        var service = new ManagementTaskService(repository, memoryCache);

        A.CallTo(() => repository.GetByIdAsync(task.Id)).Returns(task);

        var firstResult = await service.GetByIdAsync(task.Id);
        var secondResult = await service.GetByIdAsync(task.Id);

        firstResult.ShouldBe(task);
        secondResult.ShouldBe(task);
        A.CallTo(() => repository.GetByIdAsync(task.Id)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnCachedCollection_WhenCacheContainsValue()
    {
        var repository = A.Fake<IManagementTaskRepository>();
        using var memoryCache = CreateMemoryCache();
        var tasks = new[] { CreateTask() };
        memoryCache.Set("management-tasks:all", tasks);
        var service = new ManagementTaskService(repository, memoryCache);

        var result = await service.GetAllAsync();

        result.ShouldBe(tasks);
        A.CallTo(() => repository.GetAllAsync()).MustNotHaveHappened();
    }

    [Fact]
    public async Task GetAllAsync_ShouldCacheRepositoryResult_WhenCacheMissOccurs()
    {
        var repository = A.Fake<IManagementTaskRepository>();
        using var memoryCache = CreateMemoryCache();
        var tasks = new[] { CreateTask() };
        var service = new ManagementTaskService(repository, memoryCache);

        A.CallTo(() => repository.GetAllAsync()).Returns(tasks);

        var firstResult = await service.GetAllAsync();
        var secondResult = await service.GetAllAsync();

        firstResult.ShouldBe(tasks);
        secondResult.ShouldBe(tasks);
        A.CallTo(() => repository.GetAllAsync()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task UpdateAsync_ShouldInvalidateSingleTaskAndCollectionCache()
    {
        var repository = A.Fake<IManagementTaskRepository>();
        var task = CreateTask();
        using var memoryCache = CreateMemoryCache();
        memoryCache.Set(GetTaskCacheKey(task.Id), task);
        memoryCache.Set("management-tasks:all", new[] { task });
        var service = new ManagementTaskService(repository, memoryCache);

        A.CallTo(() => repository.UpdateAsync(task)).Returns(task);

        var result = await service.UpdateAsync(task);

        result.ShouldBe(task);
        memoryCache.TryGetValue(GetTaskCacheKey(task.Id), out _).ShouldBeFalse();
        memoryCache.TryGetValue("management-tasks:all", out _).ShouldBeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ShouldInvalidateSingleTaskAndCollectionCache()
    {
        var repository = A.Fake<IManagementTaskRepository>();
        var task = CreateTask();
        using var memoryCache = CreateMemoryCache();
        memoryCache.Set(GetTaskCacheKey(task.Id), task);
        memoryCache.Set("management-tasks:all", new[] { task });
        var service = new ManagementTaskService(repository, memoryCache);

        await service.DeleteAsync(task.Id);

        A.CallTo(() => repository.DeleteAsync(task.Id)).MustHaveHappenedOnceExactly();
        memoryCache.TryGetValue(GetTaskCacheKey(task.Id), out _).ShouldBeFalse();
        memoryCache.TryGetValue("management-tasks:all", out _).ShouldBeFalse();
    }

    [Fact]
    public async Task CreateAsync_ShouldInvalidateCollectionCacheAndAffectedTaskCache()
    {
        var repository = A.Fake<IManagementTaskRepository>();
        var task = CreateTask();
        using var memoryCache = CreateMemoryCache();
        memoryCache.Set(GetTaskCacheKey(task.Id), task);
        memoryCache.Set("management-tasks:all", new[] { task });
        var service = new ManagementTaskService(repository, memoryCache);

        A.CallTo(() => repository.CreateAsync(task)).Returns(task);

        await service.CreateAsync(task);

        memoryCache.TryGetValue(GetTaskCacheKey(task.Id), out _).ShouldBeFalse();
        memoryCache.TryGetValue("management-tasks:all", out _).ShouldBeFalse();
    }

    private static MemoryCache CreateMemoryCache()
    {
        return new MemoryCache(new MemoryCacheOptions());
    }

    private static string GetTaskCacheKey(Guid id)
    {
        return $"management-tasks:{id}";
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
