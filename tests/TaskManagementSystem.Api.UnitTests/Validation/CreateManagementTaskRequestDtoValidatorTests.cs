using Shouldly;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Api.Validation;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Api.UnitTests.Validation;

public sealed class CreateManagementTaskRequestDtoValidatorTests
{
    private readonly CreateManagementTaskRequestDtoValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_ShouldPass_ForValidRequest()
    {
        var request = new CreateManagementTaskRequestDto
        {
            Title = "Task title",
            Description = "Task description",
            Status = ManagementTaskStatus.Pending,
            DueDate = DateTime.UtcNow.AddDays(1),
            UserId = Guid.NewGuid()
        };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public async Task ValidateAsync_ShouldFail_ForInvalidRequest()
    {
        var request = new CreateManagementTaskRequestDto();

        var result = await _validator.ValidateAsync(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.PropertyName == nameof(CreateManagementTaskRequestDto.Title));
        result.Errors.ShouldContain(error => error.PropertyName == nameof(CreateManagementTaskRequestDto.Description));
        result.Errors.ShouldContain(error => error.PropertyName == nameof(CreateManagementTaskRequestDto.DueDate));
        result.Errors.ShouldContain(error => error.PropertyName == nameof(CreateManagementTaskRequestDto.UserId));
    }
}
