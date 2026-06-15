using Shouldly;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Api.Validation;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Api.UnitTests.Validation;

public sealed class UpdateManagementTaskRequestDtoValidatorTests
{
    private readonly UpdateManagementTaskRequestDtoValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_ShouldPass_ForValidRequest()
    {
        var request = new UpdateManagementTaskRequestDto
        {
            Id = Guid.NewGuid(),
            Title = "Task title",
            Description = "Task description",
            Status = ManagementTaskStatus.InProgress,
            DueDate = DateTime.UtcNow.AddDays(1),
            UserId = Guid.NewGuid()
        };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public async Task ValidateAsync_ShouldFail_ForInvalidRequest()
    {
        var request = new UpdateManagementTaskRequestDto();

        var result = await _validator.ValidateAsync(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.PropertyName == nameof(UpdateManagementTaskRequestDto.Id));
        result.Errors.ShouldContain(error => error.PropertyName == nameof(UpdateManagementTaskRequestDto.Title));
        result.Errors.ShouldContain(error => error.PropertyName == nameof(UpdateManagementTaskRequestDto.Description));
        result.Errors.ShouldContain(error => error.PropertyName == nameof(UpdateManagementTaskRequestDto.DueDate));
        result.Errors.ShouldContain(error => error.PropertyName == nameof(UpdateManagementTaskRequestDto.UserId));
    }

    [Fact]
    public async Task ValidateAsync_ShouldFail_ForInvalidStatus()
    {
        var request = new UpdateManagementTaskRequestDto
        {
            Id = Guid.NewGuid(),
            Title = "Task title",
            Description = "Task description",
            Status = (ManagementTaskStatus)999,
            DueDate = DateTime.UtcNow.AddDays(1),
            UserId = Guid.NewGuid()
        };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error =>
            error.PropertyName == nameof(UpdateManagementTaskRequestDto.Status) &&
            error.ErrorMessage == "Status must be a valid task status.");
    }
}
