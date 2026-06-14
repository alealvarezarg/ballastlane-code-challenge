using FluentValidation;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Api.Validation;

public sealed class UpdateManagementTaskRequestDtoValidator : AbstractValidator<UpdateManagementTaskRequestDto>
{
    public UpdateManagementTaskRequestDtoValidator()
    {
        RuleFor(request => request.Id)
            .NotEmpty()
            .WithMessage("Id must be a valid identifier.");

        RuleFor(request => request.Title)
            .NotEmpty()
            .WithMessage("Title is required.");

        RuleFor(request => request.Description)
            .NotEmpty()
            .WithMessage("Description is required.");

        RuleFor(request => request.Status)
            .Must(status => Enum.IsDefined(typeof(ManagementTaskStatus), status))
            .WithMessage("Status must be a valid task status.");

        RuleFor(request => request.DueDate)
            .NotEqual(default(DateTime))
            .WithMessage("DueDate must be a valid date.");

        RuleFor(request => request.UserId)
            .NotEmpty()
            .WithMessage("UserId must be a valid identifier.");
    }
}
