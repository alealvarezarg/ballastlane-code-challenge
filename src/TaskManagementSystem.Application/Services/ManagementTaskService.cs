using Microsoft.Extensions.Caching.Memory;
using TaskManagementSystem.Application.Abstractions;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Application.Services;

public sealed class ManagementTaskService : IManagementTaskService
{
    private const string AllTasksCacheKey = "management-tasks:all";
    private readonly IMemoryCache _memoryCache;
    private readonly IManagementTaskRepository _repository;

    public ManagementTaskService(IManagementTaskRepository repository, IMemoryCache memoryCache)
    {
        _repository = repository;
        _memoryCache = memoryCache;
    }

    public async Task<ManagementTask> CreateAsync(ManagementTask task)
    {
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

    public async Task<ManagementTask> UpdateAsync(ManagementTask task)
    {
        var updatedTask = await _repository.UpdateAsync(task);
        InvalidateCache(updatedTask.Id);
        return updatedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
        InvalidateCache(id);
    }

    private void InvalidateCache(Guid id)
    {
        _memoryCache.Remove(AllTasksCacheKey);
        _memoryCache.Remove(GetTaskByIdCacheKey(id));
    }

    private static string GetTaskByIdCacheKey(Guid id)
    {
        return $"management-tasks:{id}";
    }
}
