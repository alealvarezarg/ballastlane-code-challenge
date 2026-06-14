using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
using TaskManagementSystem.Api.Controllers;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Application.Abstractions;
using TaskManagementSystem.Application.Models;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Api.UnitTests.Controllers;

public sealed class ManagementTaskEnhancementControllerTests
{
    private readonly IManagementTaskService _managementTaskService;
    private readonly ManagementTaskController _controller;

    public ManagementTaskEnhancementControllerTests()
    {
        _managementTaskService = A.Fake<IManagementTaskService>();
        _controller = new ManagementTaskController(_managementTaskService);
    }

    [Fact]
    public async Task GetSummaryAsync_ShouldReturnOkWithSummaryPayload()
    {
        IReadOnlyCollection<ManagementTaskStatusSummary> summary =
        [
            new ManagementTaskStatusSummary
            {
                Status = ManagementTaskStatus.Pending,
                Count = 2
            }
        ];
        A.CallTo(() => _managementTaskService.GetSummaryAsync(false)).Returns(summary);

        var result = await _controller.GetSummaryAsync();

        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        var response = okResult.Value.ShouldBeOfType<ManagementTaskSummaryResponseDto>();
        response.Statuses.Single().Count.ShouldBe(2);
    }

    [Fact]
    public async Task PatchAsync_ShouldReturnUpdatedTask()
    {
        var task = CreateTask();
        A.CallTo(() => _managementTaskService.PatchAsync(task.Id, A<ManagementTaskPatch>._)).Returns(task);

        var result = await _controller.PatchAsync(task.Id, new PatchManagementTaskRequestDto { Title = "Updated" });

        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        var response = okResult.Value.ShouldBeOfType<ManagementTaskResponseDto>();
        response.Id.ShouldBe(task.Id);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldReturnUpdatedTask()
    {
        var task = CreateTask();
        A.CallTo(() => _managementTaskService.UpdateStatusAsync(task.Id, ManagementTaskStatus.InProgress)).Returns(task);

        var result = await _controller.UpdateStatusAsync(task.Id, new UpdateManagementTaskStatusRequestDto
        {
            Status = ManagementTaskStatus.InProgress
        });

        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        var response = okResult.Value.ShouldBeOfType<ManagementTaskResponseDto>();
        response.Id.ShouldBe(task.Id);
    }

    private static ManagementTask CreateTask()
    {
        return new ManagementTask(
            Guid.NewGuid(),
            "Task title",
            "Task description",
            ManagementTaskStatus.Pending,
            DateTime.UtcNow.AddDays(1),
            Guid.NewGuid());
    }
}
