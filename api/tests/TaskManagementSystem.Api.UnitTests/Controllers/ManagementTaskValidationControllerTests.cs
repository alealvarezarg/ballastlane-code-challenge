using FluentValidation;
using Shouldly;
using TaskManagementSystem.Api.Controllers;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Application.Abstractions;

namespace TaskManagementSystem.Api.UnitTests.Controllers;

public sealed class ManagementTaskValidationControllerTests
{
    private readonly ManagementTaskController _controller;

    public ManagementTaskValidationControllerTests()
    {
        _controller = new ManagementTaskController(FakeItEasy.A.Fake<IManagementTaskService>());
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowValidationException_WhenRouteAndPayloadIdsDiffer()
    {
        var request = new UpdateManagementTaskRequestDto
        {
            Id = Guid.NewGuid(),
            Title = "Task title",
            Description = "Task description",
            DueDate = DateTime.UtcNow.AddDays(1),
            UserId = Guid.NewGuid()
        };

        var exception = await Should.ThrowAsync<ValidationException>(() =>
            _controller.UpdateAsync(Guid.NewGuid(), request));

        exception.Errors.ShouldContain(error => error.PropertyName == nameof(UpdateManagementTaskRequestDto.Id));
    }

    [Fact]
    public async Task GetDueWithinAsync_ShouldThrowValidationException_WhenDaysIsNotPositive()
    {
        var exception = await Should.ThrowAsync<ValidationException>(() =>
            _controller.GetDueWithinAsync(0));

        exception.Errors.ShouldContain(error => error.PropertyName == "days");
    }
}
