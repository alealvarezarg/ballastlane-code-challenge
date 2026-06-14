using Shouldly;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Api.Validation;

namespace TaskManagementSystem.Api.UnitTests.Validation;

public sealed class PatchManagementTaskRequestDtoValidatorTests
{
    private readonly PatchManagementTaskRequestDtoValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_ShouldPass_WhenAtLeastOneFieldIsSupplied()
    {
        var result = await _validator.ValidateAsync(new PatchManagementTaskRequestDto
        {
            Title = "Updated title"
        });

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public async Task ValidateAsync_ShouldFail_WhenNoFieldsAreSupplied()
    {
        var result = await _validator.ValidateAsync(new PatchManagementTaskRequestDto());

        result.IsValid.ShouldBeFalse();
    }
}
