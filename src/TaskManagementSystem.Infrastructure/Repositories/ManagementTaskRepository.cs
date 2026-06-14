using TaskManagementSystem.Application.Abstractions;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Infrastructure.Database;

namespace TaskManagementSystem.Infrastructure.Repositories;

public sealed class ManagementTaskRepository : IManagementTaskRepository
{
    private readonly LiteDbContext _context;

    public ManagementTaskRepository(LiteDbContext context)
    {
        _context = context;
    }

    public Task<ManagementTask> CreateAsync(ManagementTask task)
    {
        var document = ManagementTaskDocument.FromDomain(task);
        _context.Tasks.Insert(document);
        return Task.FromResult(document.ToDomain());
    }

    public Task<ManagementTask?> GetByIdAsync(Guid id)
    {
        var document = _context.Tasks.FindById(id);
        return Task.FromResult(document?.ToDomain());
    }

    public Task<IReadOnlyCollection<ManagementTask>> GetAllAsync()
    {
        IReadOnlyCollection<ManagementTask> tasks = _context.Tasks
            .FindAll()
            .Select(document => document.ToDomain())
            .ToArray();

        return Task.FromResult(tasks);
    }

    public Task<ManagementTask> UpdateAsync(ManagementTask task)
    {
        var document = ManagementTaskDocument.FromDomain(task);
        var updated = _context.Tasks.Update(document);

        if (!updated)
        {
            throw new KeyNotFoundException($"ManagementTask with id '{task.Id}' was not found.");
        }

        return Task.FromResult(document.ToDomain());
    }

    public Task DeleteAsync(Guid id)
    {
        _context.Tasks.Delete(id);
        return Task.CompletedTask;
    }
}
