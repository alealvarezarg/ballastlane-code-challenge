using TaskManagementSystem.Application.Models;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Application.Abstractions;

public interface IManagementTaskRepository
{
    Task<ManagementTask> CreateAsync(ManagementTask task);

    Task<ManagementTask?> GetByIdAsync(Guid id);

    Task<IReadOnlyCollection<ManagementTask>> GetAllAsync();

    Task<PagedResult<ManagementTask>> QueryAsync(ManagementTaskQueryOptions options);

    Task<IReadOnlyCollection<ManagementTask>> GetOverdueAsync(bool includeArchived, DateTime asOfUtc);

    Task<IReadOnlyCollection<ManagementTask>> GetDueWithinAsync(int days, bool includeArchived, DateTime asOfUtc);

    Task<IReadOnlyCollection<ManagementTaskStatusSummary>> GetSummaryAsync(bool includeArchived);

    Task<ManagementTask> UpdateAsync(ManagementTask task);

    Task DeleteAsync(Guid id);

    Task<IdempotencyRecord?> GetIdempotencyRecordAsync(string key);

    Task SaveIdempotencyRecordAsync(IdempotencyRecord record);
}
