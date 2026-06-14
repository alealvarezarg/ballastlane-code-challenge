using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Api.Models;

public static class ManagementTaskMappings
{
    public static ManagementTask ToDomain(this CreateManagementTaskRequestDto request)
    {
        return new ManagementTask(
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
            UserId = task.UserId
        };
    }
}
