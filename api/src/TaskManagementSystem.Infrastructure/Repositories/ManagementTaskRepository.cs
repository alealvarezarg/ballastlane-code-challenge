using TaskManagementSystem.Application.Models;
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

    public Task<PagedResult<ManagementTask>> QueryAsync(ManagementTaskQueryOptions options)
    {
        var filteredTasks = ApplyQuery(_context.Tasks.FindAll().Select(document => document.ToDomain()), options);
        var totalCount = filteredTasks.Count();
        var page = options.Page <= 0 ? 1 : options.Page;
        var pageSize = options.PageSize <= 0 ? 50 : options.PageSize;
        var items = filteredTasks
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArray();

        return Task.FromResult(new PagedResult<ManagementTask>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        });
    }

    public Task<IReadOnlyCollection<ManagementTask>> GetOverdueAsync(bool includeArchived, DateTime asOfUtc)
    {
        IReadOnlyCollection<ManagementTask> tasks = _context.Tasks
            .FindAll()
            .Select(document => document.ToDomain())
            .Where(task => (includeArchived || !task.IsArchived) &&
                task.DueDate < asOfUtc &&
                task.Status != TaskManagementSystem.Domain.Enums.ManagementTaskStatus.Completed)
            .OrderBy(task => task.DueDate)
            .ToArray();

        return Task.FromResult(tasks);
    }

    public Task<IReadOnlyCollection<ManagementTask>> GetDueWithinAsync(int days, bool includeArchived, DateTime asOfUtc)
    {
        var endDate = asOfUtc.AddDays(days);
        IReadOnlyCollection<ManagementTask> tasks = _context.Tasks
            .FindAll()
            .Select(document => document.ToDomain())
            .Where(task => (includeArchived || !task.IsArchived) &&
                task.DueDate >= asOfUtc &&
                task.DueDate <= endDate)
            .OrderBy(task => task.DueDate)
            .ToArray();

        return Task.FromResult(tasks);
    }

    public Task<IReadOnlyCollection<ManagementTaskStatusSummary>> GetSummaryAsync(bool includeArchived)
    {
        IReadOnlyCollection<ManagementTaskStatusSummary> summary = _context.Tasks
            .FindAll()
            .Select(document => document.ToDomain())
            .Where(task => includeArchived || !task.IsArchived)
            .GroupBy(task => task.Status)
            .Select(group => new ManagementTaskStatusSummary
            {
                Status = group.Key,
                Count = group.Count()
            })
            .OrderBy(item => item.Status)
            .ToArray();

        return Task.FromResult(summary);
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

    public Task<IdempotencyRecord?> GetIdempotencyRecordAsync(string key)
    {
        var record = _context.IdempotencyRecords.FindById(key);
        return Task.FromResult(record?.ToModel());
    }

    public Task SaveIdempotencyRecordAsync(IdempotencyRecord record)
    {
        _context.IdempotencyRecords.Upsert(IdempotencyRecordDocument.FromModel(record));
        return Task.CompletedTask;
    }

    private static IEnumerable<ManagementTask> ApplyQuery(
        IEnumerable<ManagementTask> tasks,
        ManagementTaskQueryOptions options)
    {
        var query = tasks;

        if (!options.IncludeArchived)
        {
            query = query.Where(task => !task.IsArchived);
        }

        if (options.Status.HasValue)
        {
            query = query.Where(task => task.Status == options.Status.Value);
        }

        if (options.UserId.HasValue)
        {
            query = query.Where(task => task.UserId == options.UserId.Value);
        }

        if (!string.IsNullOrWhiteSpace(options.Search))
        {
            var term = options.Search.Trim();
            query = query.Where(task =>
                task.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                task.Description.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        return query.OrderBy(task => task.DueDate);
    }
}
