using TaskManagementSystem.Application.Models;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Api.Models;

public static class ManagementTaskMappings
{
    public static ManagementTask ToDomain(this CreateManagementTaskRequestDto request)
    {
        return ManagementTask.CreateNew(
            Guid.NewGuid(),
            request.Title,
            request.Description,
            request.Status,
            request.DueDate,
            request.UserId);
    }

    public static ManagementTask ToDomain(this UpdateManagementTaskRequestDto request)
    {
        return new ManagementTask(
            request.Id,
            request.Title,
            request.Description,
            request.Status,
            request.DueDate,
            request.UserId);
    }

    public static ManagementTaskResponseDto ToResponseDto(this ManagementTask task)
    {
        return new ManagementTaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            DueDate = task.DueDate,
            UserId = task.UserId,
            IsArchived = task.IsArchived,
            ArchivedAt = task.ArchivedAt
        };
    }

    public static ManagementTaskPatch ToPatch(this PatchManagementTaskRequestDto request)
    {
        return new ManagementTaskPatch
        {
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            UserId = request.UserId
        };
    }

    public static PagedManagementTaskResponseDto ToResponseDto(this PagedResult<ManagementTask> result)
    {
        return new PagedManagementTaskResponseDto
        {
            Items = result.Items.Select(task => task.ToResponseDto()).ToArray(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }

    public static ManagementTaskSummaryResponseDto ToResponseDto(this IReadOnlyCollection<ManagementTaskStatusSummary> summary)
    {
        return new ManagementTaskSummaryResponseDto
        {
            Statuses = summary
                .Select(item => new ManagementTaskStatusCountDto
                {
                    Status = item.Status,
                    Count = item.Count
                })
                .ToArray()
        };
    }
}
