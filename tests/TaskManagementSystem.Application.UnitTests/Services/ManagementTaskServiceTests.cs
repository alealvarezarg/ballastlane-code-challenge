using FakeItEasy;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Shouldly;
using TaskManagementSystem.Application.Abstractions;
using TaskManagementSystem.Application.Models;
using TaskManagementSystem.Application.Services;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;
using TaskManagementSystem.Domain.Exceptions;

namespace TaskManagementSystem.Application.UnitTests.Services;

public sealed class ManagementTaskServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldCallRepositoryAndReturnCreatedTask()
    {
        var repository = A.Fake<IManagementTaskRepository>();
        var managementUserRepository = A.Fake<IManagementUserRepository>();
        var logger = A.Fake<ILogger<ManagementTaskService>>();
        using var memoryCache = CreateMemoryCache();
        var service = new ManagementTaskService(repository, memoryCache, managementUserRepository, logger);
        var task = CreateTask();

        A.CallTo(() => repository.GetIdempotencyRecordAsync(A<string>._)).Returns(Task.FromResult<IdempotencyRecord?>(null));
        A.CallTo(() => managementUserRepository.ExistsAsync(task.UserId)).Returns(true);
        A.CallTo(() => repository.CreateAsync(task)).Returns(task);

        var result = await service.CreateAsync(task);

        result.ShouldBe(task);
        A.CallTo(() => repository.CreateAsync(task)).MustHaveHappenedOnceExactly();
        A.CallTo(() => repository.SaveIdempotencyRecordAsync(A<IdempotencyRecord>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task CreateAsync_ShouldGenerateGuidIdempotencyKey_WhenKeyIsNotProvided()
    {
        var repository = A.Fake<IManagementTaskRepository>();
        var managementUserRepository = A.Fake<IManagementUserRepository>();
        var logger = A.Fake<ILogger<ManagementTaskService>>();
        using var memoryCache = CreateMemoryCache();
        var service = new ManagementTaskService(repository, memoryCache, managementUserRepository, logger);
        var task = CreateTask();
        IdempotencyRecord? savedRecord = null;

        A.CallTo(() => repository.GetIdempotencyRecordAsync(A<string>._)).Returns(Task.FromResult<IdempotencyRecord?>(null));
        A.CallTo(() => managementUserRepository.ExistsAsync(task.UserId)).Returns(true);
        A.CallTo(() => repository.CreateAsync(task)).Returns(task);
        A.CallTo(() => repository.SaveIdempotencyRecordAsync(A<IdempotencyRecord>._))
            .Invokes(call => savedRecord = call.GetArgument<IdempotencyRecord>(0));

        await service.CreateAsync(task);

        var record = savedRecord.ShouldNotBeNull();
        Guid.TryParse(record.Key, out _).ShouldBeTrue();
        record.TaskId.ShouldBe(task.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCachedTask_WhenCacheContainsValue()
    {
        var repository = A.Fake<IManagementTaskRepository>();
        var managementUserRepository = A.Fake<IManagementUserRepository>();
        var logger = A.Fake<ILogger<ManagementTaskService>>();
        var task = CreateTask();
        using var memoryCache = CreateMemoryCache();
        memoryCache.Set(GetTaskCacheKey(task.Id), task);
        var service = new ManagementTaskService(repository, memoryCache, managementUserRepository, logger);

        var result = await service.GetByIdAsync(task.Id);

        result.ShouldBe(task);
        A.CallTo(() => repository.GetByIdAsync(task.Id)).MustNotHaveHappened();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldCacheRepositoryResult_WhenCacheMissOccurs()
    {
        var repository = A.Fake<IManagementTaskRepository>();
        var managementUserRepository = A.Fake<IManagementUserRepository>();
        var logger = A.Fake<ILogger<ManagementTaskService>>();
        var task = CreateTask();
        using var memoryCache = CreateMemoryCache();
        var service = new ManagementTaskService(repository, memoryCache, managementUserRepository, logger);

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
        var managementUserRepository = A.Fake<IManagementUserRepository>();
        var logger = A.Fake<ILogger<ManagementTaskService>>();
        using var memoryCache = CreateMemoryCache();
        var tasks = new[] { CreateTask() };
        memoryCache.Set("management-tasks:all", tasks);
        var service = new ManagementTaskService(repository, memoryCache, managementUserRepository, logger);

        var result = await service.GetAllAsync();

        result.ShouldBe(tasks);
        A.CallTo(() => repository.GetAllAsync()).MustNotHaveHappened();
    }

    [Fact]
    public async Task GetAllAsync_ShouldCacheRepositoryResult_WhenCacheMissOccurs()
    {
        var repository = A.Fake<IManagementTaskRepository>();
        var managementUserRepository = A.Fake<IManagementUserRepository>();
        var logger = A.Fake<ILogger<ManagementTaskService>>();
        using var memoryCache = CreateMemoryCache();
        var tasks = new[] { CreateTask() };
        var service = new ManagementTaskService(repository, memoryCache, managementUserRepository, logger);

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
        var managementUserRepository = A.Fake<IManagementUserRepository>();
        var logger = A.Fake<ILogger<ManagementTaskService>>();
        var task = CreateTask();
        using var memoryCache = CreateMemoryCache();
        memoryCache.Set(GetTaskCacheKey(task.Id), task);
        memoryCache.Set("management-tasks:all", new[] { task });
        var service = new ManagementTaskService(repository, memoryCache, managementUserRepository, logger);
        var existingTask = new ManagementTask(
            task.Id,
            task.Title,
            task.Description,
            task.Status,
            task.DueDate,
            task.UserId);

        A.CallTo(() => repository.GetByIdAsync(task.Id)).Returns(existingTask);
        A.CallTo(() => repository.UpdateAsync(A<ManagementTask>._))
            .ReturnsLazily(call => Task.FromResult(call.GetArgument<ManagementTask>(0)!));

        var result = await service.UpdateAsync(task);

        result.Id.ShouldBe(task.Id);
        memoryCache.TryGetValue(GetTaskCacheKey(task.Id), out _).ShouldBeFalse();
        memoryCache.TryGetValue("management-tasks:all", out _).ShouldBeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ShouldInvalidateSingleTaskAndCollectionCache()
    {
        var repository = A.Fake<IManagementTaskRepository>();
        var managementUserRepository = A.Fake<IManagementUserRepository>();
        var logger = A.Fake<ILogger<ManagementTaskService>>();
        var task = CreateTask();
        using var memoryCache = CreateMemoryCache();
        memoryCache.Set(GetTaskCacheKey(task.Id), task);
        memoryCache.Set("management-tasks:all", new[] { task });
        var service = new ManagementTaskService(repository, memoryCache, managementUserRepository, logger);
        A.CallTo(() => repository.GetByIdAsync(task.Id)).Returns(task);
        A.CallTo(() => repository.UpdateAsync(A<ManagementTask>._))
            .ReturnsLazily(call => Task.FromResult(call.GetArgument<ManagementTask>(0)!));

        await service.DeleteAsync(task.Id);

        A.CallTo(() => repository.UpdateAsync(A<ManagementTask>._)).MustHaveHappenedOnceExactly();
        memoryCache.TryGetValue(GetTaskCacheKey(task.Id), out _).ShouldBeFalse();
        memoryCache.TryGetValue("management-tasks:all", out _).ShouldBeFalse();
    }

    [Fact]
    public async Task CreateAsync_ShouldInvalidateCollectionCacheAndAffectedTaskCache()
    {
        var repository = A.Fake<IManagementTaskRepository>();
        var managementUserRepository = A.Fake<IManagementUserRepository>();
        var logger = A.Fake<ILogger<ManagementTaskService>>();
        var task = CreateTask();
        using var memoryCache = CreateMemoryCache();
        memoryCache.Set(GetTaskCacheKey(task.Id), task);
        memoryCache.Set("management-tasks:all", new[] { task });
        var service = new ManagementTaskService(repository, memoryCache, managementUserRepository, logger);

        A.CallTo(() => repository.GetIdempotencyRecordAsync(A<string>._)).Returns(Task.FromResult<IdempotencyRecord?>(null));
        A.CallTo(() => managementUserRepository.ExistsAsync(task.UserId)).Returns(true);
        A.CallTo(() => repository.CreateAsync(task)).Returns(task);

        await service.CreateAsync(task);

        memoryCache.TryGetValue(GetTaskCacheKey(task.Id), out _).ShouldBeFalse();
        memoryCache.TryGetValue("management-tasks:all", out _).ShouldBeFalse();
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenManagementUserDoesNotExist()
    {
        var repository = A.Fake<IManagementTaskRepository>();
        var managementUserRepository = A.Fake<IManagementUserRepository>();
        var logger = A.Fake<ILogger<ManagementTaskService>>();
        using var memoryCache = CreateMemoryCache();
        var service = new ManagementTaskService(repository, memoryCache, managementUserRepository, logger);
        var task = CreateTask();
        A.CallTo(() => repository.GetIdempotencyRecordAsync(A<string>._)).Returns(Task.FromResult<IdempotencyRecord?>(null));
        A.CallTo(() => managementUserRepository.ExistsAsync(task.UserId)).Returns(false);

        var exception = await Should.ThrowAsync<ManagementUserNotFoundException>(() => service.CreateAsync(task));

        exception.Message.ShouldContain(task.UserId.ToString());
        A.CallTo(() => repository.CreateAsync(A<ManagementTask>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task CreateAsync_ShouldWriteInformationLog_WhenTaskIsCreated()
    {
        var repository = A.Fake<IManagementTaskRepository>();
        var managementUserRepository = A.Fake<IManagementUserRepository>();
        var logger = A.Fake<ILogger<ManagementTaskService>>();
        using var memoryCache = CreateMemoryCache();
        var service = new ManagementTaskService(repository, memoryCache, managementUserRepository, logger);
        var task = CreateTask();

        A.CallTo(() => repository.GetIdempotencyRecordAsync(A<string>._)).Returns(Task.FromResult<IdempotencyRecord?>(null));
        A.CallTo(() => managementUserRepository.ExistsAsync(task.UserId)).Returns(true);
        A.CallTo(() => repository.CreateAsync(task)).Returns(task);

        await service.CreateAsync(task);

        A.CallTo(logger).Where(call =>
            call.Method.Name == nameof(ILogger.Log) &&
            call.GetArgument<LogLevel>(0) == LogLevel.Information &&
            call.GetArgument<object>(2).ToString()!.Contains(task.Id.ToString(), StringComparison.Ordinal))
            .MustHaveHappened();
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
