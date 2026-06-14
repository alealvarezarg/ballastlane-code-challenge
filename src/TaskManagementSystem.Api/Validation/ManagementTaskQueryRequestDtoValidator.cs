using FluentValidation;
using TaskManagementSystem.Api.Models;

namespace TaskManagementSystem.Api.Validation;

public sealed class ManagementTaskQueryRequestDtoValidator : AbstractValidator<ManagementTaskQueryRequestDto>
{
    private static readonly string[] AllowedSortFields = ["title", "status", "dueDate"];
    private static readonly string[] AllowedSortDirections = ["asc", "desc"];

    public ManagementTaskQueryRequestDtoValidator()
    {
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
