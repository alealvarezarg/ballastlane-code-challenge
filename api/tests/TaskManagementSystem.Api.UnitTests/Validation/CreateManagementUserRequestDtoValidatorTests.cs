using Shouldly;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Api.Validation;

namespace TaskManagementSystem.Api.UnitTests.Validation;

public sealed class CreateManagementUserRequestDtoValidatorTests
{
    private readonly CreateManagementUserRequestDtoValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_ShouldPass_ForValidRequest()
    {
        var request = new CreateManagementUserRequestDto
        {
            Username = "manager",
            Email = "manager@example.com",
            Password = "Password123!"
        };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public async Task ValidateAsync_ShouldFail_ForInvalidRequest()
    {
        var request = new CreateManagementUserRequestDto
        {
            Email = "invalid-email",
            Password = "short"
        };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.PropertyName == nameof(CreateManagementUserRequestDto.Username));
        result.Errors.ShouldContain(error => error.PropertyName == nameof(CreateManagementUserRequestDto.Email));
        result.Errors.ShouldContain(error => error.PropertyName == nameof(CreateManagementUserRequestDto.Password));
    }
}
