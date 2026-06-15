using Shouldly;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Api.Validation;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Api.UnitTests.Validation;

public sealed class UpdateManagementTaskStatusRequestDtoValidatorTests
{
    private readonly UpdateManagementTaskStatusRequestDtoValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_ShouldPass_ForValidStatus()
    {
        var result = await _validator.ValidateAsync(new UpdateManagementTaskStatusRequestDto
        {
            Status = ManagementTaskStatus.InProgress
        });

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public async Task ValidateAsync_ShouldFail_ForInvalidStatus()
    {
        var result = await _validator.ValidateAsync(new UpdateManagementTaskStatusRequestDto
        {
            Status = (ManagementTaskStatus)999
        });

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error =>
            error.PropertyName == nameof(UpdateManagementTaskStatusRequestDto.Status) &&
            error.ErrorMessage == "Status must be a valid task status.");
    }
}
