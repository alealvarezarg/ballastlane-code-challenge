using FluentValidation;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Api.Validation;

public sealed class UpdateManagementTaskStatusRequestDtoValidator : AbstractValidator<UpdateManagementTaskStatusRequestDto>
{
    public UpdateManagementTaskStatusRequestDtoValidator()
    {
        RuleFor(request => request.Status)
            .Must(status => Enum.IsDefined(typeof(ManagementTaskStatus), status))
            .WithMessage("Status must be a valid task status.");
    }
}
