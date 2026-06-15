using Shouldly;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Api.Validation;

namespace TaskManagementSystem.Api.UnitTests.Validation;

public sealed class LoginManagementUserRequestDtoValidatorTests
{
    private readonly LoginManagementUserRequestDtoValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_ShouldPass_ForValidRequest()
    {
        var request = new LoginManagementUserRequestDto
        {
            Email = "manager@example.com",
            Password = "Password123!"
        };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public async Task ValidateAsync_ShouldFail_ForInvalidRequest()
    {
        var request = new LoginManagementUserRequestDto
        {
            Email = "invalid-email"
        };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.PropertyName == nameof(LoginManagementUserRequestDto.Email));
        result.Errors.ShouldContain(error => error.PropertyName == nameof(LoginManagementUserRequestDto.Password));
    }
}
