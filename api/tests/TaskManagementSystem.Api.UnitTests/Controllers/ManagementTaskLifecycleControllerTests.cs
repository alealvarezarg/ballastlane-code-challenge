using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
using TaskManagementSystem.Api.Controllers;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Application.Abstractions;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Api.UnitTests.Controllers;

public sealed class ManagementTaskLifecycleControllerTests
{
    private readonly IManagementTaskService _managementTaskService;
    private readonly ManagementTaskController _controller;

    public ManagementTaskLifecycleControllerTests()
    {
        _managementTaskService = A.Fake<IManagementTaskService>();
        _controller = new ManagementTaskController(_managementTaskService);
    }

    [Fact]
    public async Task CreateAsync_ShouldDefaultMissingStatusToPending()
    {
        var request = new CreateManagementTaskRequestDto
        {
            Title = "Task title",
            Description = "Task description",
            DueDate = DateTime.UtcNow.AddDays(1),
            UserId = Guid.NewGuid()
        };
        ManagementTask? createdTask = null;
        A.CallTo(() => _managementTaskService.CreateAsync(A<ManagementTask>._, A<string?>._))
            .Invokes(call => createdTask = call.GetArgument<ManagementTask>(0))
            .ReturnsLazily(call =>
                Task.FromResult(call.GetArgument<ManagementTask>(0)));

        await _controller.CreateAsync(request, "idem-1");

        createdTask.ShouldNotBeNull();
        createdTask.Status.ShouldBe(ManagementTaskStatus.Pending);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnNoContent_WhenArchiveSucceeds()
    {
        var id = Guid.NewGuid();
        A.CallTo(() => _managementTaskService.GetByIdAsync(id)).Returns(new ManagementTask(
            id,
            "Task title",
            "Task description",
            ManagementTaskStatus.Pending,
            DateTime.UtcNow.AddDays(1),
            Guid.NewGuid()));

        var result = await _controller.DeleteAsync(id);

        result.ShouldBeOfType<NoContentResult>();
        A.CallTo(() => _managementTaskService.DeleteAsync(id)).MustHaveHappenedOnceExactly();
    }
}
