using FluentValidation;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Api.Validation;

public sealed class ManagementTaskQueryRequestDtoValidator : AbstractValidator<ManagementTaskQueryRequestDto>
{
    public ManagementTaskQueryRequestDtoValidator()
    {
        When(request => request.Status.HasValue, () =>
        {
            RuleFor(request => request.Status!.Value)
                .Must(status => Enum.IsDefined(typeof(ManagementTaskStatus), status))
                .WithMessage("Status must be a valid task status.")
                .OverridePropertyName(nameof(ManagementTaskQueryRequestDto.Status));
        });

        RuleFor(request => request.Page)
            .GreaterThan(0);

        RuleFor(request => request.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(200);
    }
}
