using TaskManagementSystem.Application.Models;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.Abstractions;

public interface IManagementTaskService
{
    Task<ManagementTask> CreateAsync(ManagementTask task, string? idempotencyKey = null);

    Task<ManagementTask?> GetByIdAsync(Guid id);

    Task<IReadOnlyCollection<ManagementTask>> GetAllAsync();

    Task<PagedResult<ManagementTask>> QueryAsync(ManagementTaskQueryOptions options);

    Task<IReadOnlyCollection<ManagementTask>> GetOverdueAsync(bool includeArchived, DateTime asOfUtc);

    Task<IReadOnlyCollection<ManagementTask>> GetDueWithinAsync(int days, bool includeArchived, DateTime asOfUtc);

    Task<IReadOnlyCollection<ManagementTaskStatusSummary>> GetSummaryAsync(bool includeArchived);

    Task<ManagementTask> UpdateAsync(ManagementTask task);

    Task<ManagementTask> PatchAsync(Guid id, ManagementTaskPatch patch);

    Task<ManagementTask> UpdateStatusAsync(Guid id, ManagementTaskStatus status);

    Task DeleteAsync(Guid id);
}
