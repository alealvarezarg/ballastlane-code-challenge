using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<ManagementTaskService> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly IManagementTaskRepository _repository;
    private readonly IManagementUserRepository _managementUserRepository;

    public ManagementTaskService(
        IManagementTaskRepository repository,
        IMemoryCache memoryCache,
        IManagementUserRepository managementUserRepository,
        ILogger<ManagementTaskService> logger)
    {
        _repository = repository;
        _memoryCache = memoryCache;
        _managementUserRepository = managementUserRepository;
        _logger = logger;
    }

    public async Task<ManagementTask> CreateAsync(ManagementTask task, string? idempotencyKey = null)
    {
        var effectiveIdempotencyKey = string.IsNullOrWhiteSpace(idempotencyKey)
            ? Guid.NewGuid().ToString("D")
            : idempotencyKey;

        var existingRecord = await _repository.GetIdempotencyRecordAsync(effectiveIdempotencyKey);
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

        await EnsureManagementUserExistsAsync(task.UserId);
        var createdTask = await _repository.CreateAsync(task);
        await _repository.SaveIdempotencyRecordAsync(new IdempotencyRecord
        {
            Key = effectiveIdempotencyKey,
            RequestFingerprint = fingerprint,
            TaskId = createdTask.Id,
            CreatedAt = DateTime.UtcNow
        });

        InvalidateCache(createdTask.Id);
        _logger.LogInformation(
            "Created management task {TaskId} for user {UserId} using idempotency key {IdempotencyKey}.",
            createdTask.Id,
            createdTask.UserId,
            effectiveIdempotencyKey);
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
        _logger.LogInformation("Updated management task {TaskId}.", updatedTask.Id);
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
        _logger.LogInformation("Patched management task {TaskId}.", id);
        return updatedTask;
    }

    public async Task<ManagementTask> UpdateStatusAsync(Guid id, ManagementTaskStatus status)
    {
        var existingTask = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"ManagementTask with id '{id}' was not found.");

        existingTask.ChangeStatus(status);

        var updatedTask = await _repository.UpdateAsync(existingTask);
        InvalidateCache(id);
        _logger.LogInformation("Updated management task {TaskId} status to {Status}.", id, status);
        return updatedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var existingTask = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"ManagementTask with id '{id}' was not found.");

        existingTask.Archive(DateTime.UtcNow);
        await _repository.UpdateAsync(existingTask);
        InvalidateCache(id);
        _logger.LogInformation("Archived management task {TaskId}.", id);
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

    private async Task EnsureManagementUserExistsAsync(Guid userId)
    {
        if (await _managementUserRepository.ExistsAsync(userId))
        {
            return;
        }

        throw new ManagementUserNotFoundException($"ManagementUser with id '{userId}' does not exist.");
    }
}
