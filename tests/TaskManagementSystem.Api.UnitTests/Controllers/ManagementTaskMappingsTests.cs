using Shouldly;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Api.UnitTests.Controllers;

public sealed class ManagementTaskMappingsTests
{
    [Fact]
    public void CreateRequest_ShouldMapToDomainTask()
    {
        var request = new CreateManagementTaskRequestDto
        {
            Title = "Task title",
            Description = "Task description",
            Status = ManagementTaskStatus.Pending,
            DueDate = DateTime.UtcNow.AddDays(1),
            UserId = Guid.NewGuid()
        };

        var result = request.ToDomain();

        result.Id.ShouldNotBe(Guid.Empty);
        result.Title.ShouldBe(request.Title);
        result.Description.ShouldBe(request.Description);
        result.Status.ShouldBe(request.Status!.Value);
        result.DueDate.ShouldBe(request.DueDate);
        result.UserId.ShouldBe(request.UserId);
    }

    [Fact]
    public void CreateRequest_ShouldDefaultStatusToPending_WhenStatusIsOmitted()
    {
        var request = new CreateManagementTaskRequestDto
        {
            Title = "Task title",
            Description = "Task description",
            DueDate = DateTime.UtcNow.AddDays(1),
            UserId = Guid.NewGuid()
        };

        var result = request.ToDomain();

        result.Status.ShouldBe(ManagementTaskStatus.Pending);
    }

    [Fact]
    public void UpdateRequest_ShouldMapToDomainTask()
    {
        var request = new UpdateManagementTaskRequestDto
        {
            Id = Guid.NewGuid(),
            Title = "Task title",
            Description = "Task description",
            Status = ManagementTaskStatus.InProgress,
            DueDate = DateTime.UtcNow.AddDays(2),
            UserId = Guid.NewGuid()
        };

        var result = request.ToDomain();

        result.Id.ShouldBe(request.Id);
        result.Title.ShouldBe(request.Title);
        result.Description.ShouldBe(request.Description);
        result.Status.ShouldBe(request.Status);
        result.DueDate.ShouldBe(request.DueDate);
        result.UserId.ShouldBe(request.UserId);
    }

    [Fact]
    public void DomainTask_ShouldMapToResponseDto()
    {
        var task = new ManagementTask(
            Guid.NewGuid(),
            "Task title",
            "Task description",
            ManagementTaskStatus.Completed,
            DateTime.UtcNow.AddDays(3),
            Guid.NewGuid());

        var result = task.ToResponseDto();

        result.Id.ShouldBe(task.Id);
        result.Title.ShouldBe(task.Title);
        result.Description.ShouldBe(task.Description);
        result.Status.ShouldBe(task.Status);
        result.DueDate.ShouldBe(task.DueDate);
        result.UserId.ShouldBe(task.UserId);
        result.IsArchived.ShouldBe(task.IsArchived);
    }
}
