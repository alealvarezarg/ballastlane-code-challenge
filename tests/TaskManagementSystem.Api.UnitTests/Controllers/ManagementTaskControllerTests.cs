using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
using TaskManagementSystem.Api.Controllers;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Application.Abstractions;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Api.UnitTests.Controllers;

public sealed class ManagementTaskControllerTests
{
    private readonly IManagementTaskService _managementTaskService;
    private readonly ManagementTaskController _controller;

    public ManagementTaskControllerTests()
    {
        _managementTaskService = A.Fake<IManagementTaskService>();
        _controller = new ManagementTaskController(_managementTaskService);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOkWithResponseDtos()
    {
        var tasks = new[]
        {
            CreateTask(),
            CreateTask()
        };
        A.CallTo(() => _managementTaskService.GetAllAsync()).Returns(tasks);

        var result = await _controller.GetAllAsync();

        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        var response = okResult.Value.ShouldBeAssignableTo<IReadOnlyCollection<ManagementTaskResponseDto>>();
        response!.Count.ShouldBe(2);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOkWithResponseDto()
    {
        var task = CreateTask();
        A.CallTo(() => _managementTaskService.GetByIdAsync(task.Id)).Returns(task);

        var result = await _controller.GetByIdAsync(task.Id);

        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        var response = okResult.Value.ShouldBeOfType<ManagementTaskResponseDto>();
        response.Id.ShouldBe(task.Id);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnCreatedAtAction()
    {
        var request = new CreateManagementTaskRequestDto
        {
            Title = "Task title",
            Description = "Task description",
            Status = ManagementTaskStatus.Pending,
            DueDate = DateTime.UtcNow.AddDays(1),
            UserId = Guid.NewGuid()
        };
        ManagementTask? createdTask = null;

        A.CallTo(() => _managementTaskService.CreateAsync(A<ManagementTask>._))
            .Invokes(call => createdTask = call.GetArgument<ManagementTask>(0))
            .ReturnsLazily(() => createdTask!);

        var result = await _controller.CreateAsync(request);

        var createdResult = result.Result.ShouldBeOfType<CreatedAtActionResult>();
        createdResult.ActionName.ShouldBe(nameof(ManagementTaskController.GetByIdAsync));
        var response = createdResult.Value.ShouldBeOfType<ManagementTaskResponseDto>();
        response.Title.ShouldBe(request.Title);
        createdTask.ShouldNotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnOkWithUpdatedResponseDto()
    {
        var request = new UpdateManagementTaskRequestDto
        {
            Id = Guid.NewGuid(),
            Title = "Updated title",
            Description = "Updated description",
            Status = ManagementTaskStatus.InProgress,
            DueDate = DateTime.UtcNow.AddDays(2),
            UserId = Guid.NewGuid()
        };
        var updatedTask = request.ToDomain();
        A.CallTo(() => _managementTaskService.UpdateAsync(A<ManagementTask>._)).Returns(updatedTask);

        var result = await _controller.UpdateAsync(request.Id, request);

        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        var response = okResult.Value.ShouldBeOfType<ManagementTaskResponseDto>();
        response.Id.ShouldBe(request.Id);
        response.Title.ShouldBe(request.Title);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnNoContent_WhenTaskExists()
    {
        var task = CreateTask();
        A.CallTo(() => _managementTaskService.GetByIdAsync(task.Id)).Returns(task);

        var result = await _controller.DeleteAsync(task.Id);

        result.ShouldBeOfType<NoContentResult>();
        A.CallTo(() => _managementTaskService.DeleteAsync(task.Id)).MustHaveHappenedOnceExactly();
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
