using FluentValidation;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Api.Validation;

public sealed class ManagementTaskQueryRequestDtoValidator : AbstractValidator<ManagementTaskQueryRequestDto>
{
    private static readonly string[] AllowedSortFields = ["title", "status", "dueDate"];
    private static readonly string[] AllowedSortDirections = ["asc", "desc"];

    public ManagementTaskQueryRequestDtoValidator()
    {
        When(request => request.Status.HasValue, () =>
        {
            RuleFor(request => request.Status!.Value)
                .Must(status => Enum.IsDefined(typeof(ManagementTaskStatus), status))
                .WithMessage("Status must be a valid task status.")
                .OverridePropertyName(nameof(ManagementTaskQueryRequestDto.Status));
        });

        RuleFor(request => request.SortBy)
            .Must(value => string.IsNullOrWhiteSpace(value) || AllowedSortFields.Contains(value))
            .WithMessage("SortBy must be one of title, status, or dueDate.");

        RuleFor(request => request.SortDirection)
            .Must(value => string.IsNullOrWhiteSpace(value) || AllowedSortDirections.Contains(value))
            .WithMessage("SortDirection must be asc or desc.");

        RuleFor(request => request.Page)
            .GreaterThan(0);

        RuleFor(request => request.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(200);
    }
}
