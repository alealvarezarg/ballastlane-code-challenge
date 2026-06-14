using FluentValidation;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Api.Validation;

public sealed class CreateManagementTaskRequestDtoValidator : AbstractValidator<CreateManagementTaskRequestDto>
{
    public CreateManagementTaskRequestDtoValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty();

        RuleFor(request => request.Description)
            .NotEmpty();

        RuleFor(request => request.Status)
            .Must(status => Enum.IsDefined(typeof(ManagementTaskStatus), status))
            .WithMessage("Status must be a valid task status.");

        RuleFor(request => request.DueDate)
            .NotEqual(default(DateTime))
            .WithMessage("DueDate must be a valid date.");

        RuleFor(request => request.UserId)
            .NotEmpty();
    }
}
