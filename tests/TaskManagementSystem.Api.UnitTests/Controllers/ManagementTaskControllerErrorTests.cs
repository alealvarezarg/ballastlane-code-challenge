using FakeItEasy;
using FluentValidation;
using Shouldly;
using TaskManagementSystem.Api.Controllers;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Application.Abstractions;

namespace TaskManagementSystem.Api.UnitTests.Controllers;

public sealed class ManagementTaskControllerErrorTests
{
    private readonly IManagementTaskService _managementTaskService;
    private readonly ManagementTaskController _controller;

    public ManagementTaskControllerErrorTests()
    {
        _managementTaskService = A.Fake<IManagementTaskService>();
        _controller = new ManagementTaskController(_managementTaskService);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowKeyNotFoundException_WhenTaskDoesNotExist()
    {
        var id = Guid.NewGuid();
        A.CallTo(() => _managementTaskService.GetByIdAsync(id)).Returns(Task.FromResult<TaskManagementSystem.Domain.Entities.ManagementTask?>(null));

        var exception = await Should.ThrowAsync<KeyNotFoundException>(() => _controller.GetByIdAsync(id));

        exception.Message.ShouldContain(id.ToString());
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowValidationException_WhenRouteIdDoesNotMatchBodyId()
    {
        var request = new UpdateManagementTaskRequestDto
        {
            Id = Guid.NewGuid(),
            Title = "Task title",
            Description = "Task description",
            DueDate = DateTime.UtcNow.AddDays(1),
            UserId = Guid.NewGuid()
        };

        var exception = await Should.ThrowAsync<ValidationException>(() => _controller.UpdateAsync(Guid.NewGuid(), request));

        exception.Errors.ShouldContain(error => error.PropertyName == nameof(UpdateManagementTaskRequestDto.Id));
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowKeyNotFoundException_WhenTaskDoesNotExist()
    {
        var id = Guid.NewGuid();
        A.CallTo(() => _managementTaskService.GetByIdAsync(id)).Returns(Task.FromResult<TaskManagementSystem.Domain.Entities.ManagementTask?>(null));

        var exception = await Should.ThrowAsync<KeyNotFoundException>(() => _controller.DeleteAsync(id));

        exception.Message.ShouldContain(id.ToString());
    }
}
