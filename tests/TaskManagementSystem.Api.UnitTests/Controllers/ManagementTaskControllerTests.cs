using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManagementSystem.Api.Controllers;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Application.Models;
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
    public async Task GetAllAsync_ShouldReturnOkWithPagedResponseDto()
    {
        var tasks = new PagedResult<ManagementTask>
        {
            Items = new[]
            {
                CreateTask(),
                CreateTask()
            },
            Page = 1,
            PageSize = 50,
            TotalCount = 2
        };
        A.CallTo(() => _managementTaskService.QueryAsync(A<ManagementTaskQueryOptions>._)).Returns(tasks);

        var result = await _controller.GetAllAsync(new ManagementTaskQueryRequestDto());

        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        var response = okResult.Value.ShouldBeOfType<PagedManagementTaskResponseDto>();
        response.Items.Count.ShouldBe(2);
        response.TotalCount.ShouldBe(2);
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
    public async Task CreateAsync_ShouldReturnCreatedAtRoute()
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

        A.CallTo(() => _managementTaskService.CreateAsync(A<ManagementTask>._, A<string?>._))
            .Invokes(call => createdTask = call.GetArgument<ManagementTask>(0))
            .ReturnsLazily(() => createdTask!);

        var result = await _controller.CreateAsync(request, null);

        var createdResult = result.Result.ShouldBeOfType<CreatedAtRouteResult>();
        createdResult.RouteName.ShouldBe("GetManagementTaskById");
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

    [Fact]
    public async Task GetOverdueAsync_ShouldReturnOkWithTaskResponseDtos()
    {
        var tasks = new[]
        {
            CreateTask(),
            CreateTask()
        };
        A.CallTo(() => _managementTaskService.GetOverdueAsync(false, A<DateTime>._)).Returns(tasks);

        var result = await _controller.GetOverdueAsync();

        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        var response = okResult.Value.ShouldBeOfType<ManagementTaskResponseDto[]>();
        response.Length.ShouldBe(2);
    }

    [Fact]
    public async Task GetSummaryAsync_ShouldReturnOkWithSummaryResponseDto()
    {
        var summary = new[]
        {
            new ManagementTaskStatusSummary
            {
                Status = ManagementTaskStatus.Pending,
                Count = 2
            }
        };
        A.CallTo(() => _managementTaskService.GetSummaryAsync(false)).Returns(summary);

        var result = await _controller.GetSummaryAsync();

        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        var response = okResult.Value.ShouldBeOfType<ManagementTaskSummaryResponseDto>();
        response.Statuses.Count.ShouldBe(1);
        response.Statuses.First().ShouldBeOfType<ManagementTaskStatusCountDto>();
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldReturnOkWithResponseDto()
    {
        var task = CreateTask();
        var request = new UpdateManagementTaskStatusRequestDto
        {
            Status = ManagementTaskStatus.Completed
        };
        A.CallTo(() => _managementTaskService.UpdateStatusAsync(task.Id, request.Status)).Returns(task);

        var result = await _controller.UpdateStatusAsync(task.Id, request);

        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBeOfType<ManagementTaskResponseDto>();
    }

    [Fact]
    public void CreateRequestDto_ShouldDeserializeStringStatus()
    {
        const string json = """
            {
              "title": "Task title",
              "description": "Task description",
              "status": "Completed",
              "dueDate": "2030-01-01T00:00:00Z",
              "userId": "11111111-1111-1111-1111-111111111111"
            }
            """;

        var result = JsonSerializer.Deserialize<CreateManagementTaskRequestDto>(json, CreateApiJsonOptions());

        result.ShouldNotBeNull();
        result.Status.ShouldBe(ManagementTaskStatus.Completed);
    }

    [Fact]
    public void UpdateStatusRequestDto_ShouldDeserializeStringStatus()
    {
        const string json = """
            {
              "status": "InProgress"
            }
            """;

        var result = JsonSerializer.Deserialize<UpdateManagementTaskStatusRequestDto>(json, CreateApiJsonOptions());

        result.ShouldNotBeNull();
        result.Status.ShouldBe(ManagementTaskStatus.InProgress);
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

    private static JsonSerializerOptions CreateApiJsonOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        options.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
        return options;
    }
}
