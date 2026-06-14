using Shouldly;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Api.Validation;

namespace TaskManagementSystem.Api.UnitTests.Validation;

public sealed class ManagementTaskQueryRequestDtoValidatorTests
{
    private readonly ManagementTaskQueryRequestDtoValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_ShouldPass_ForValidRequest()
    {
        var result = await _validator.ValidateAsync(new ManagementTaskQueryRequestDto
        {
            SortBy = "title",
            SortDirection = "asc",
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
            SortBy = "invalid",
            SortDirection = "sideways",
            Page = 0,
            PageSize = 0
        });

        result.IsValid.ShouldBeFalse();
    }
}
