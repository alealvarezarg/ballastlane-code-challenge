using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Application.Abstractions;

public interface IManagementTaskRepository
{
    Task<ManagementTask> CreateAsync(ManagementTask task);

    Task<ManagementTask?> GetByIdAsync(Guid id);

    Task<IReadOnlyCollection<ManagementTask>> GetAllAsync();

    Task<ManagementTask> UpdateAsync(ManagementTask task);

    Task DeleteAsync(Guid id);
}
