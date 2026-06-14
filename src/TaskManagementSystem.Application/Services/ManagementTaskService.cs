using Microsoft.Extensions.Caching.Memory;
using TaskManagementSystem.Application.Abstractions;
using TaskManagementSystem.Application.Models;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;
using TaskManagementSystem.Domain.Exceptions;

namespace TaskManagementSystem.Application.Services;

public sealed class ManagementTaskService : IManagementTaskService
{
    private const string AllTasksCacheKey = "management-tasks:all";
    private const string SummaryCacheKey = "management-tasks:summary";
    private const string OverdueCacheKey = "management-tasks:overdue";
    private const string DueWithinCacheKey = "management-tasks:due-within";
    private const string CacheGenerationKey = "management-tasks:cache-generation";
    private readonly IMemoryCache _memoryCache;
    private readonly IManagementTaskRepository _repository;

    public ManagementTaskService(IManagementTaskRepository repository, IMemoryCache memoryCache)
    {
        _repository = repository;
        _memoryCache = memoryCache;
    }

    public async Task<ManagementTask> CreateAsync(ManagementTask task, string? idempotencyKey = null)
    {
        if (!string.IsNullOrWhiteSpace(idempotencyKey))
        {
            var existingRecord = await _repository.GetIdempotencyRecordAsync(idempotencyKey);
            var fingerprint = CreateFingerprint(task);

            if (existingRecord is not null)
            {
                if (!string.Equals(existingRecord.RequestFingerprint, fingerprint, StringComparison.Ordinal))
                {
                    throw new ManagementTaskLifecycleException(
                        "The supplied idempotency key has already been used for a different create request.");
                }

                var existingTask = await _repository.GetByIdAsync(existingRecord.TaskId)
                    ?? throw new KeyNotFoundException($"ManagementTask with id '{existingRecord.TaskId}' was not found.");

                return existingTask;
            }

            var idempotentCreatedTask = await _repository.CreateAsync(task);
            await _repository.SaveIdempotencyRecordAsync(new IdempotencyRecord
            {
                Key = idempotencyKey,
                RequestFingerprint = fingerprint,
                TaskId = idempotentCreatedTask.Id,
                CreatedAt = DateTime.UtcNow
            });

            InvalidateCache(idempotentCreatedTask.Id);
            return idempotentCreatedTask;
        }

        var createdTask = await _repository.CreateAsync(task);
        InvalidateCache(createdTask.Id);
        return createdTask;
    }

    public async Task<ManagementTask?> GetByIdAsync(Guid id)
    {
        return await _memoryCache.GetOrCreateAsync(
            GetTaskByIdCacheKey(id),
            async _ => await _repository.GetByIdAsync(id));
    }

    public async Task<IReadOnlyCollection<ManagementTask>> GetAllAsync()
    {
        return await _memoryCache.GetOrCreateAsync(
            AllTasksCacheKey,
            async _ => await _repository.GetAllAsync()) ?? Array.Empty<ManagementTask>();
    }

    public async Task<PagedResult<ManagementTask>> QueryAsync(ManagementTaskQueryOptions options)
    {
        var cacheKey = GetQueryCacheKey(options);

        return await _memoryCache.GetOrCreateAsync(
            $"{GetCacheGeneration()}:{cacheKey}",
            async _ => await _repository.QueryAsync(options)) ?? new PagedResult<ManagementTask>
        {
            Items = Array.Empty<ManagementTask>(),
            Page = options.Page,
            PageSize = options.PageSize,
            TotalCount = 0
        };
    }

    public async Task<IReadOnlyCollection<ManagementTask>> GetOverdueAsync(bool includeArchived, DateTime asOfUtc)
    {
        return await _memoryCache.GetOrCreateAsync(
            $"{GetCacheGeneration()}:{OverdueCacheKey}:{includeArchived}:{asOfUtc:O}",
            async _ => await _repository.GetOverdueAsync(includeArchived, asOfUtc)) ?? Array.Empty<ManagementTask>();
    }

    public async Task<IReadOnlyCollection<ManagementTask>> GetDueWithinAsync(int days, bool includeArchived, DateTime asOfUtc)
    {
        return await _memoryCache.GetOrCreateAsync(
            $"{GetCacheGeneration()}:{DueWithinCacheKey}:{days}:{includeArchived}:{asOfUtc:O}",
            async _ => await _repository.GetDueWithinAsync(days, includeArchived, asOfUtc)) ?? Array.Empty<ManagementTask>();
    }

    public async Task<IReadOnlyCollection<ManagementTaskStatusSummary>> GetSummaryAsync(bool includeArchived)
    {
        return await _memoryCache.GetOrCreateAsync(
            $"{GetCacheGeneration()}:{SummaryCacheKey}:{includeArchived}",
            async _ => await _repository.GetSummaryAsync(includeArchived)) ?? Array.Empty<ManagementTaskStatusSummary>();
    }

    public async Task<ManagementTask> UpdateAsync(ManagementTask task)
    {
        var existingTask = await _repository.GetByIdAsync(task.Id)
            ?? throw new KeyNotFoundException($"ManagementTask with id '{task.Id}' was not found.");

        existingTask.ApplyFullUpdate(
            task.Title,
            task.Description,
            task.Status,
            task.DueDate,
            task.UserId);

        var updatedTask = await _repository.UpdateAsync(existingTask);
        InvalidateCache(updatedTask.Id);
        return updatedTask;
    }

    public async Task<ManagementTask> PatchAsync(Guid id, ManagementTaskPatch patch)
    {
        var existingTask = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"ManagementTask with id '{id}' was not found.");

        existingTask.PatchInformation(
            patch.Title,
            patch.Description,
            patch.DueDate,
            patch.UserId);

        var updatedTask = await _repository.UpdateAsync(existingTask);
        InvalidateCache(id);
        return updatedTask;
    }

    public async Task<ManagementTask> UpdateStatusAsync(Guid id, ManagementTaskStatus status)
    {
        var existingTask = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"ManagementTask with id '{id}' was not found.");

        existingTask.ChangeStatus(status);

        var updatedTask = await _repository.UpdateAsync(existingTask);
        InvalidateCache(id);
        return updatedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var existingTask = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"ManagementTask with id '{id}' was not found.");

        existingTask.Archive(DateTime.UtcNow);
        await _repository.UpdateAsync(existingTask);
        InvalidateCache(id);
    }

    private void InvalidateCache(Guid id)
    {
        _memoryCache.Remove(AllTasksCacheKey);
        _memoryCache.Remove(GetTaskByIdCacheKey(id));
        _memoryCache.Set(CacheGenerationKey, Guid.NewGuid().ToString("N"));
    }

    private static string GetTaskByIdCacheKey(Guid id)
    {
        return $"management-tasks:{id}";
    }

    private static string GetQueryCacheKey(ManagementTaskQueryOptions options)
    {
        return
            $"management-tasks:query:{options.Status}:{options.UserId}:{options.Search}:{options.SortBy}:{options.SortDirection}:{options.Page}:{options.PageSize}:{options.IncludeArchived}";
    }

    private static string CreateFingerprint(ManagementTask task)
    {
        return string.Join(
            "|",
            task.Title,
            task.Description,
            task.Status,
            task.DueDate.ToUniversalTime().ToString("O"),
            task.UserId);
    }

    private string GetCacheGeneration()
    {
        if (_memoryCache.TryGetValue<string>(CacheGenerationKey, out var cacheGeneration) &&
            !string.IsNullOrWhiteSpace(cacheGeneration))
        {
            return cacheGeneration;
        }

        cacheGeneration = Guid.NewGuid().ToString("N");
        _memoryCache.Set(CacheGenerationKey, cacheGeneration);
        return cacheGeneration;
    }
}
