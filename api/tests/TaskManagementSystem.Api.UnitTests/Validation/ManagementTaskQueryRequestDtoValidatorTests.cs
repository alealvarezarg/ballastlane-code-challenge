using Shouldly;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Api.Validation;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Api.UnitTests.Validation;

public sealed class ManagementTaskQueryRequestDtoValidatorTests
{
    private readonly ManagementTaskQueryRequestDtoValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_ShouldPass_ForValidRequest()
    {
        var result = await _validator.ValidateAsync(new ManagementTaskQueryRequestDto
        {
            Page = 1,
            PageSize = 20
        });

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public async Task ValidateAsync_ShouldFail_ForInvalidRequest()
    {
        var result = await _validator.ValidateAsync(new ManagementTaskQueryRequestDto
        {
            Page = 0,
            PageSize = 0
        });

        result.IsValid.ShouldBeFalse();
    }

    [Fact]
    public async Task ValidateAsync_ShouldFail_ForInvalidStatus()
    {
        var result = await _validator.ValidateAsync(new ManagementTaskQueryRequestDto
        {
            Status = (ManagementTaskStatus)999,
            Page = 1,
            PageSize = 20
        });

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error =>
            error.PropertyName == nameof(ManagementTaskQueryRequestDto.Status) &&
            error.ErrorMessage == "Status must be a valid task status.");
    }
}
