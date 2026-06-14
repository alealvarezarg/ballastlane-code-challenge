using FakeItEasy;
using Microsoft.Extensions.Caching.Memory;
using Shouldly;
using TaskManagementSystem.Application.Abstractions;
using TaskManagementSystem.Application.Models;
using TaskManagementSystem.Application.Services;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.UnitTests.Services;

public sealed class ManagementTaskServiceEnhancementTests
{
    [Fact]
    public async Task QueryAsync_ShouldCacheRepositoryResult_ForSameQuery()
    {
        var repository = A.Fake<IManagementTaskRepository>();
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var service = new ManagementTaskService(repository, memoryCache);
        var options = new ManagementTaskQueryOptions
        {
            Status = ManagementTaskStatus.Pending,
            Page = 1,
            PageSize = 10
        };
        var result = new PagedResult<ManagementTask>
        {
            Items = [CreateTask()],
            Page = 1,
            PageSize = 10,
            TotalCount = 1
        };

        A.CallTo(() => repository.QueryAsync(A<ManagementTaskQueryOptions>._)).Returns(result);

        var first = await service.QueryAsync(options);
        var second = await service.QueryAsync(options);

        first.TotalCount.ShouldBe(1);
        second.TotalCount.ShouldBe(1);
        A.CallTo(() => repository.QueryAsync(A<ManagementTaskQueryOptions>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnExistingTask_WhenMatchingIdempotencyKeyExists()
    {
        var repository = A.Fake<IManagementTaskRepository>();
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var service = new ManagementTaskService(repository, memoryCache);
        var task = CreateTask();
        var key = "idem-1";

        A.CallTo(() => repository.GetIdempotencyRecordAsync(key)).Returns(new IdempotencyRecord
        {
            Key = key,
            RequestFingerprint = string.Join("|", task.Title, task.Description, task.Status, task.DueDate.ToUniversalTime().ToString("O"), task.UserId),
            TaskId = task.Id,
            CreatedAt = DateTime.UtcNow
        });
        A.CallTo(() => repository.GetByIdAsync(task.Id)).Returns(task);

        var result = await service.CreateAsync(task, key);

        result.Id.ShouldBe(task.Id);
        A.CallTo(() => repository.CreateAsync(A<ManagementTask>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task DeleteAsync_ShouldArchiveExistingTask_UsingRepositoryUpdate()
    {
        var repository = A.Fake<IManagementTaskRepository>();
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var service = new ManagementTaskService(repository, memoryCache);
        var task = CreateTask();

        A.CallTo(() => repository.GetByIdAsync(task.Id)).Returns(task);
        A.CallTo(() => repository.UpdateAsync(A<ManagementTask>._))
            .Invokes(call => call.GetArgument<ManagementTask>(0).IsArchived.ShouldBeTrue())
            .ReturnsLazily(call => Task.FromResult(call.GetArgument<ManagementTask>(0)));

        await service.DeleteAsync(task.Id);

        A.CallTo(() => repository.UpdateAsync(A<ManagementTask>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => repository.DeleteAsync(A<Guid>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task GetSummaryAsync_ShouldCacheRepositoryResult_ForSameScope()
    {
        var repository = A.Fake<IManagementTaskRepository>();
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var service = new ManagementTaskService(repository, memoryCache);
        IReadOnlyCollection<ManagementTaskStatusSummary> summary =
        [
            new ManagementTaskStatusSummary
            {
                Status = ManagementTaskStatus.Pending,
                Count = 2
            }
        ];

        A.CallTo(() => repository.GetSummaryAsync(false)).Returns(summary);

        await service.GetSummaryAsync(false);
        await service.GetSummaryAsync(false);

        A.CallTo(() => repository.GetSummaryAsync(false)).MustHaveHappenedOnceExactly();
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
